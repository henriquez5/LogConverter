using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogConverter.Utils
{
    public class InvalidLogFormatException : Exception
    {
        public InvalidLogFormatException(string message) : base(message)
        {
        }
    }
}
