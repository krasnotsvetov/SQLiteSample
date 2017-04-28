using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    [Command("help", "show help")]
    public class HelpCommand : ICommand
    {
        private Dictionary<string, string> commands = new Dictionary<string, string>();

        public HelpCommand(string[] args)
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
            Console.WriteLine("Available commands:");
            foreach (var kvp in commands)
            {
                Console.WriteLine($"\tCommand name : {kvp.Key}");
                if (!kvp.Value.Equals("")) Console.WriteLine($"\tCommand description : {kvp.Value}");
                Console.WriteLine();
            }
            return CommandResult.Success;
        }
    }
}
