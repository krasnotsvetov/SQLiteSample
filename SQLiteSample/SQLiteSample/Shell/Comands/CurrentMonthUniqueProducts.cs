using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [Command("curMonthUniqueProducts", "show products which was ordered in this month, but not in previous")]
    public class CurrentMonthUniqueProducts : ICommand
    {
        public CurrentMonthUniqueProducts(string[] args)
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
                            command.CommandText = $@"SELECT prod.name FROM
                                                    (
	                                                    SELECT firstSET.product_id FROM
	                                                    (
		                                                    SELECT DISTINCT(ord.product_id)
		                                                    FROM {context.DataBase.Orders.Name} ord 
		                                                    WHERE strftime('%m', ord.dt) = strftime('%m', current_date) 

	                                                    ) firstSET
	                                                    LEFT JOIN 
	                                                    (
		                                                    SELECT DISTINCT(ord.product_id)
		                                                    FROM {context.DataBase.Orders.Name} ord
		                                                    WHERE strftime('%m', ord.dt) = strftime('%m', datetime('now', '-1 month'))
	                                                    ) secondSET
	                                                    ON firstSET.product_id = secondSET.product_id
	                                                    WHERE secondSET.product_id IS NULL
                                                    ) uniqProdIDs
                                                    JOIN {context.DataBase.Products.Name} prod ON prod.id = uniqProdIDs.product_id";
                            var reader = command.ExecuteReader();
                            Console.WriteLine("Product");
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader[0]}");
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
