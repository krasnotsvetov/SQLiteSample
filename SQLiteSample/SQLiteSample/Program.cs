using SQLiteSample.Shell;
using SQLiteSample.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: <file_name>");
                Console.WriteLine("file_name : is a path to file, which will be used to initialize a table");
            }
            try
            {
                string fileName = Path.GetFileName(args[0]);
                if (fileName.Equals(string.Empty))
                {
                    Console.WriteLine($"{args[0]} is not a file");
                }
                else
                {
                    Console.WriteLine("--- Init---");
                    using (var db = new DataBase(fileName))
                    {
                        db.CreateProducts();
                        db.CreateOrdersTable(args[0]);
                        new ShellManager(db).Run();
                    }
                }
            } catch (ArgumentException e)
            {
                Console.WriteLine("The path is incorrect");
            } catch (Exception e)
            {
                Console.WriteLine("Unhandled exception");
                Console.WriteLine(e.Message);

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
