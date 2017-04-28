using SQLiteSample.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteSample.Shell.Comands
{
    /// <summary>
    /// Interface for shell command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute shell command.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        CommandResult Invoke(CommandContext context);
    }

    public enum CommandResult
    {
        Success,
        Failed,
        Exit,
    }
}
