using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lolcomjector
{
    public class InvalidDllPath : Exception
    {
        public InvalidDllPath(string message)
            : base(message)
        {

        }
    }
    public class InvalidProcess : Exception
    {
        public InvalidProcess(string message)
            : base(message)
        {

        }

    }
    public class AlreadyInjected : Exception
    {
        public AlreadyInjected(string message)
            : base(message)
        {

        }
    }
}
