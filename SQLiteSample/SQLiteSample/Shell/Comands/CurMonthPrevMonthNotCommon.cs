using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [Command("curMonthPrevMonthNotCommon", "show orders which was ordered at previous month or in current, but not in both" )]
    public class CurMonthPrevMonthNotCommon : ICommand
    {
        public CurMonthPrevMonthNotCommon(string[] args)
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

                            //SQLite doesn't support FULL OUTER JOIN
                            //So, take records for two month. - A
                            //    take records which common for two month via inner join  - B
                            //    calculate A / B via left join
                            command.CommandText = $@"SELECT prod.name FROM
                                                    (
	                                                    SELECT allPart.product_id FROM
	                                                    (
		                                                    SELECT DISTINCT(ord.product_id)
		                                                    FROM {context.DataBase.Orders.Name} ord 
		                                                    WHERE strftime('%m', ord.dt) = strftime('%m', current_date) 
		                                                       OR strftime('%m', ord.dt) = strftime('%m', datetime('now', '-1 month'))
	                                                    ) allPart
	                                                    LEFT JOIN 
	                                                    (
		                                                    SELECT firstSET.product_id FROM
		                                                    (
			                                                    SELECT DISTINCT(ord.product_id)
			                                                    FROM {context.DataBase.Orders.Name} ord 
			                                                    WHERE strftime('%m', ord.dt) = strftime('%m', current_date) 
		                                                    ) firstSET
		                                                    INNER JOIN 
		                                                    (
			                                                    SELECT DISTINCT(ord.product_id)
			                                                    FROM {context.DataBase.Orders.Name} ord
			                                                    WHERE strftime('%m', ord.dt) = strftime('%m', datetime('now', '-1 month'))
		                                                    ) secondSET
		                                                    ON firstSET.product_id = secondSET.product_id
	                                                    ) commonPart
	                                                    ON allPart.product_id = commonPart.product_id
	                                                    WHERE commonPart.product_id IS NULL
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
