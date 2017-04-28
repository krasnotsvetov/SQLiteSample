using SQLiteSample.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{

    /// <summary>
    /// A context whish is using for command executing
    /// </summary>
    public class CommandContext
    {
        public DataBase DataBase { get; private set; }
        public CommandContext(DataBase dataBase)
        {
            this.DataBase = dataBase;
        }
    }
}
