using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.Core
{
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
