using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [Command("monthTopProduct", "show top products for each month")]
    public class MonthTopProduct : ICommand
    {
        public MonthTopProduct(string[] args)
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
                            command.CommandText = $@"SELECT prod.name AS ProductName, strftime('%m-%Y', finalTable.OrderDate), 
                                                            finalTable.TotalAmount, finalTable.Part FROM
	                                                (
	                                                    SELECT tempTable.product_id,tempTable.dt as OrderDate, MAX(tempTable.AMOUNT), tempTable.AMOUNT as TotalAmount, tempTable.AMOUNT / SUM(tempTable.AMOUNT) * 100 as Part FROM
		                                                (
		                                                    SELECT ord.product_id, MAX(ord.amount) AS MAXIMUM, ord.dt as dt, SUM(amount) AS AMOUNT FROM {context.DataBase.Orders.Name} ord
		                                                    GROUP BY strftime('%m-%Y', ord.dt), ord.product_id
		                                                ) tempTable
	                                                    GROUP BY strftime('%m-%Y', tempTable.dt)
	                                                ) finalTable
                                                    JOIN {context.DataBase.Products.Name} prod ON prod.id = finalTable.product_id
                                                    ORDER BY finalTable.OrderDate";
                            var reader = command.ExecuteReader();
                            Console.WriteLine("Product\tDate\tAmount\tPart");
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]} {reader[3]}");
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
