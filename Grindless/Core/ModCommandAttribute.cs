using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless
{
    /// <summary>
    /// Registers a <see cref="CommandParser"/> instance or static method for automatic adding.
    /// Use <see cref="CommandEntry.AutoAddModCommands"/> during loading to add such methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModCommandAttribute : Attribute
    {
        public ModCommandAttribute(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command must be a non-empty string.");

            Command = command;
        }

        public string Command { get; }
    }
}
