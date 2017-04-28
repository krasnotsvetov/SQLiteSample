using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Tables
{
    public class TableManager : IDisposable
    {
        /// <summary>
        /// A delegate for MakeTransaction method
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public delegate bool TransactionFunction(SQLiteTransaction transaction);

        /// <summary>
        /// A delegate for MakeConnection method
        /// </summary>
        /// <param name="connection"></param>
        public delegate void ConnectionFunction(SQLiteConnection connection);

        public string Name { get; private set; }

        private DataBase db;

        public TableManager(string name, DataBase db)
        {
            this.db = db;
            this.Name = name;
        }


        /// <summary>
        /// Create a table
        /// </summary>
        /// <param name="columns">names and types</param>
        public void CreateTable(Dictionary<string, string> columns, string addition = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"CREATE TABLE {Name} (");
            foreach (var kvp in columns)
            {
                sb.Append($"{kvp.Key} {kvp.Value},");
            }
            //remove last comma
            if (addition.Equals(""))
            {
                sb.Remove(sb.Length - 1, 1);
            } else
            {
                sb.Append(addition);
            }
            sb.Append(");");

            using (var connection = new SQLiteConnection($"Data source = {db.DataBaseName}").OpenAndReturn())
            {
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = $"DROP TABLE IF EXISTS {Name}";
                    command.ExecuteNonQuery();
                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }


        /// <summary>
        /// Make transaction and execute transactionFunction via this connection
        /// </summary>
        /// <param name="transactionFunction"></param>
        public void MakeTransaction(TransactionFunction transactionFunction)
        {
            using (var connection = new SQLiteConnection($"Data source = {db.DataBaseName}").OpenAndReturn())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    bool result = transactionFunction?.Invoke(transaction) ?? false;
                    if (result)
                    {
                        transaction.Commit();
                    }
                    else transaction.Rollback();
                }
                connection.Close();
            }
        }


        /// <summary>
        /// Create connection and execute connectionFunction via this connection
        /// </summary>
        /// <param name="connectionFunction"></param>
        public void MakeConnection(ConnectionFunction connectionFunction)
        {
            using (var connection = new SQLiteConnection($"Data source = {db.DataBaseName}").OpenAndReturn())
            {
                connectionFunction?.Invoke(connection);
                connection.Close();
            }
        }

        public void Dispose()
        {
        }
    }
}
