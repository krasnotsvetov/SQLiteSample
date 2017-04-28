using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [Command("enumerateMonthProducts", "list of products with the quantity and amount of the order for the current month.")]
    public class EnumerateMonthProducts : ICommand
    {
        public EnumerateMonthProducts(string[] args)
        {
        }

        public CommandResult Invoke(CommandContext context)
        {
            try
            {
                context.DataBase.Orders.MakeConnection(
                    (connection) =>
                    {
                        using (var command = new SQLiteCommand(connection))
                        {
                            command.CommandText = $@"SELECT  prod.name, SUM(ord.amount), COUNT(ord.amount)
                                                    FROM {context.DataBase.Orders.Name} ord
                                                      JOIN {context.DataBase.Products.Name} prod ON prod.id = ord.product_id
                                                    WHERE strftime('%m-%Y', ord.dt) = strftime('%m-%Y', current_date) 
                                                    GROUP BY prod.name";
                            var reader = command.ExecuteReader();
                            Console.WriteLine("Product\tAmount\tNumber of orders");
                            while (reader.Read())   
                            {
                                Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                            }
                        }
                    }
                );
            } catch
            {
                return CommandResult.Failed;
            }

            return CommandResult.Success;
        }
    }
}
