using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public class MainChampion : Champion
    {
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

        public MainChampion(int id, int baseAddr)
            : base(id, baseAddr)
        {

        }
    }
}
