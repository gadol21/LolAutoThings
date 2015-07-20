using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public struct Camera
    {
        public float CameraX { get; set; }
        public float CameraY { get; set; }
        public float CameraZ { get; set; }
        public float CameraAngle { get; set; }
        public float CameraFovY { get; set; }
        public override string ToString()
        {
            return "X: "+CameraX+" Y: "+CameraY+" Z: "+CameraZ+" Angle: "+CameraAngle+" FovY "+CameraFovY;
        }
    }
}
