using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string CommandName { get; private set; }
        public string Description { get; private set; }
        public CommandAttribute(string name, string description = "")
        {
            this.CommandName = name;
            this.Description = description;
        }

    }
}
