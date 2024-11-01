using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Exceptions
{
    public class DefaultException : Exception
    {
        public DefaultException(string msg) : base(msg) { }
    }
}
