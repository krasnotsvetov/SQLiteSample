using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [Command("clear", "clear console")]
    public class ClearCommand : ICommand
    {
        private Dictionary<string, string> commands = new Dictionary<string, string>();

        public ClearCommand(string[] args)
        {
            var cTypes = Assembly.GetEntryAssembly().GetTypes().Where(t => t.GetCustomAttribute<CommandAttribute>() != null).ToList();
            cTypes.ForEach(
                a =>
                {
                    var t = a.GetCustomAttribute<CommandAttribute>();
                    commands[t.CommandName] = t.Description;
                }
            );
        }

        public CommandResult Invoke(CommandContext context)
        {
            Console.Clear();
            return CommandResult.Success;
        }
    }
}
