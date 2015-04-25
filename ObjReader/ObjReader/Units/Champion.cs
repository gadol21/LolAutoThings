using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public class Champion : Unit
    {
        public bool isAI { get { return name.EndsWith(" bot"); } } //probably not a good idea

        public int level
        {
            get
            {
                int levelStructStart = Memory.ReadInt(Engine.processHandle, (int)Engine.moduleHandle + Offsets.Level.baseOffset, buffer);
                int add = Memory.ReadInt(Engine.processHandle, levelStructStart + Offsets.Level.offset0, buffer);
                int level = Memory.ReadInt(Engine.processHandle, add + Offsets.Level.level, buffer);
                return level;
            }
        }

        internal Champion(int id, int baseAddr)
            : base(id,baseAddr)
        {

        }
    }
}
