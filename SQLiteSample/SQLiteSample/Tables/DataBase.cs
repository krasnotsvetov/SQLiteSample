using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Tables
{
    public class DataBase : IDisposable
    {

        public TableManager Orders { get; private set; }
        public TableManager Products { get; private set; }
        public String DataBaseName { get; private set; }

        private const string productsTable = "products.in";
        private int productsCount = 0;

        private const int batchSize = 1000;


        public DataBase(string name)
        {
            this.DataBaseName = name.Split('.')[0] + ".sqlite";
        }


        /// <summary>
        /// Initialize orders table
        /// </summary>
        /// <param name="sourcePath">A path to raw data for orders table</param>
        public void CreateOrdersTable(string sourcePath)
        {
            ///
            /// A funtion, which check corectness of line in raw data file.
            ///
            bool IsRowCorrect(string id, string dateTime, string productID, string amount)
            {
                return int.TryParse(id, out int r0) && DateTime.TryParse(dateTime, out DateTime r1) &&
                       float.TryParse(amount, out float r2) && int.TryParse(productID, out int r3) && r3 <= productsCount && r3 > 0;
            }

            using (var sw = new StreamReader(new FileStream(sourcePath, FileMode.Open)))
            {
                var header = sw.ReadLine();
                var columnsNames = header.Split('\t');
                Dictionary<string, int> columnPos = new Dictionary<string, int>();
                if (columnsNames.Length != 4)
                {
                    Console.WriteLine("The header line of file is incorrect. Using default names");
                    columnsNames = new string[] { "id", "dt", "product_id", "amount" };
                    columnPos = new Dictionary<string, int>()
                    {
                        {"id", 0},
                        {"dt", 1 },
                        {"product_id", 2},
                        {"amount", 3 }
                    };  
                }
                for (int i = 0; i < 4; i++)
                {
                    columnPos[columnsNames[i]] = i;
                }

                Dictionary<string, string> types = new Dictionary<string, string>()
                {
                    {"id", "int" },
                    {"dt", "datetime" },
                    {"product_id", "int" },
                    {"amount", "real" }
                };


                //for speed checking and logging
                var stopWatch = new Stopwatch();
                int lineIndex = 1;
                stopWatch.Start();
                //Initialize table and make 
                Orders = new TableManager("Orders", this);
                Orders.CreateTable(types, $"FOREIGN KEY (product_id) REFERENCES {Products.Name}(id)");

                Orders.MakeTransaction(
                    (transaction) =>
                    {
                        using (var command = new SQLiteCommand(transaction.Connection))
                        {
                            string line;
                            int batchCount = 0;
                            var commandBuilder = new StringBuilder();
                            commandBuilder.Append($"INSERT INTO {Orders.Name} (id, dt, product_id, amount) VALUES");

                            while ((line = sw.ReadLine()) != null)
                            {
                                lineIndex++;
                                var elements = line.Split('\t');
                                var elementsCount = elements.Length;
                                if (elementsCount != 4)
                                {
                                    Console.WriteLine($"Line {lineIndex} is incorrect. The number of columns is wrong");
                                    continue;
                                }
                                if (IsRowCorrect(elements[columnPos["id"]], 
                                                 elements[columnPos["dt"]],
                                                 elements[columnPos["product_id"]],
                                                 elements[columnPos["amount"]]))
                                {

                                } else
                                {
                                    Console.WriteLine($"Line {lineIndex} is incorrect. The format is wrong");
                                    continue;
                                }

                                commandBuilder.Append($"('{elements[columnPos["id"]]}', '{elements[columnPos["dt"]]}'," +
                                                      $" '{elements[columnPos["product_id"]]}', '{elements[columnPos["amount"]]}'),");

                                batchCount++;
                                if (batchCount == batchSize)
                                {
                                    batchCount = 0;
                                    commandBuilder.Remove(commandBuilder.Length - 1, 1);
                                    command.CommandText = commandBuilder.ToString();
                                    command.ExecuteNonQuery();
                                    commandBuilder.Clear();
                                    commandBuilder.Append($"INSERT INTO {Orders.Name} (id, dt, product_id, amount) VALUES");
                                }
                            }

                            if (batchCount != 0)
                            {
                                commandBuilder.Remove(commandBuilder.Length - 1, 1);
                                command.CommandText = commandBuilder.ToString();
                                command.ExecuteNonQuery();
                            }
                        }
                        return true;
                    }
                );
                stopWatch.Stop();
                Console.WriteLine($"Adding {lineIndex} lines in {stopWatch.ElapsedMilliseconds} ms.");

            }
        }

        /// <summary>
        /// Initialize product table
        /// </summary>
        public void CreateProducts()
        {
            Dictionary<string, string> types = new Dictionary<string, string>()
                {
                    {"id", "int" },
                    {"name", "text" },
                };

            Products = new TableManager("Products", this);
            Products.CreateTable(types);

            using (var sw = new StreamReader(new FileStream(productsTable, FileMode.Open)))
            {
                string line = "";
                Products.MakeConnection(
                    (connection) =>
                    {
                        using (var command = new SQLiteCommand(connection))
                        {
                            while ((line = sw.ReadLine()) != null)
                            {
                                productsCount++;
                                var temp = line.Split('\t');
                                command.CommandText = $"INSERT INTO {Products.Name} (id, name) VALUES ({temp[0]}, '{temp[1]}')";
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                );

            }
        }


        public void Dispose()
        {

        }
    }
}
