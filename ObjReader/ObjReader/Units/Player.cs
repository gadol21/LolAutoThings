using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public class Player : Unit
    {
        public bool isAI { get; private set; }
        internal Player(int id)
            : base(id)
        {

        }
    }
}
