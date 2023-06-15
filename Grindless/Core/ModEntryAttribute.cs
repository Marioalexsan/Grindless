using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal class ModEntryAttribute : Attribute
{
    public ModEntryAttribute(long start, long count = 1000)
    {
        Start = start;
        Count = count;
    }

    public long Start { get; }
    public long Count { get; }
}
