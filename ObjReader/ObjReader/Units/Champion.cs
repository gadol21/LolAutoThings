using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public class Champion : Unit
    {
        public bool isAI { get { return name.EndsWith(" bot"); } } //probably not a good idea
        internal Champion(int id)
            : base(id)
        {

        }
    }
}
