using SQLiteSample.Shell.Comands;
using SQLiteSample.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell
{
    public class ShellManager
    {
        private Dictionary<string, Type> commandsType = new Dictionary<string, Type>();
        private HelpCommand helpCommand;
        private DataBase dataBase;

        /// <summary>
        /// Initialize a shell manager which allows to interact with database by commands.
        /// </summary>
        public ShellManager(DataBase db)
        {
            this.dataBase = db;

            ///Add all commands to shell
            /// To create a command, create a class which will have custom attribute : CommandAttribute and implement ICommand interface
            var cTypes = Assembly.GetEntryAssembly().GetTypes().Where(t => t.GetCustomAttribute<CommandAttribute>() != null).ToList();
            cTypes.ForEach(a => commandsType.Add(a.GetCustomAttribute<CommandAttribute>().CommandName, a));

            helpCommand = new HelpCommand(new string[] { "HelpCommand" });
        }

        /// <summary>
        /// Start main loop of application
        /// </summary>
        public void Run()
        {
            bool isRunning = true;
            Console.WriteLine("--- Init success---");
            Console.WriteLine("Type 'help' for more information");
            while (isRunning)
            {
                var commandArgs = Console.ReadLine().Split(' ');
                if (!commandsType.ContainsKey(commandArgs[0]))
                {
                    helpCommand?.Invoke(null);
                    continue;
                }

                var command = (ICommand)Activator.CreateInstance(commandsType[commandArgs[0]], new object[] { commandArgs});
                switch (command.Invoke(new CommandContext(dataBase)))
                {
                    case CommandResult.Exit:
                        isRunning = false;
                        break;
                    case CommandResult.Failed:

                        break;
                }
            }
        }
    }
}
