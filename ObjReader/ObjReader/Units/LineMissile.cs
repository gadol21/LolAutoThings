using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public class LineMissile : Unit
    {
        public float originX { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.originX, buffer); } }
        public float originZ { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.originZ, buffer); } }
        public float originY { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.originY, buffer); } }

        public float endX { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.endX, buffer); } }
        public float endZ { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.endZ, buffer); } }
        public float endY { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.endY, buffer); } }

        public float currentX { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.currentX, buffer); } }
        public float currentZ { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.currentZ, buffer); } }
        public float currentY { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.currentY, buffer); } }

        public float lineWidth { get { return Memory.ReadFloat(Engine.processHandle, (int)baseAddr + Offsets.LineMissile.lineWidth, buffer); } }

        internal LineMissile(int idInList, int baseAddr)
            : base(idInList, baseAddr)
        {

        }
    }
}
