using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using lolcomjector;

namespace ObjectReader
{
    public static class LoLReader
    {
        private static List<Unit> units = new List<Unit>();
        private static IntPtr processHandle, moduleHandle;
        private static void Loop()
        {
            byte[] buffer = new byte[4];

            while (Win32.FindWindow(null, "League of Legends (TM) Client") == IntPtr.Zero)
            {
                Console.WriteLine("No league" + "," + DateTime.Now);
                System.Threading.Thread.Sleep(1000);
            }
            Process[] ps = Process.GetProcessesByName("League of Legends");
            Process p = ps[0];
            processHandle = Win32.OpenProcess((int)Win32.ProcessAccessFlags.All, false, p.Id);
            IntPtr ptr = Win32.CreateToolhelp32Snapshot(Win32.SnapshotFlags.Module, Convert.ToUInt32(p.Id));
            Win32.MODULEENTRY32 module = new Win32.MODULEENTRY32();
            unsafe
            {
                module.dwSize = Convert.ToUInt32(Marshal.SizeOf(module));
            }
            Win32.Module32First(ptr, ref module);
            while (module.szModule != "League of Legends.exe")
            {
                Win32.Module32Next(ptr, ref module);
            }
            moduleHandle = module.modBaseAddr;
            int objsNum = 0x9C00 / 4;
            byte[] buff = new byte[4];
            //ReadProcessMemory((int)processHandle, (int)moduleHandle + 0x02980638, buffer, 4, ref bytesRead);
            int listStart = 0;
            while (listStart == 0)
            {
                listStart = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.ObjectList.ListBegin, buffer);
                Thread.Sleep(100);
            }
            while (true)
            {
                List<Unit> localUnits = new List<Unit>();
                for (int i = 0; i < objsNum; i++)
                {
                    //ReadProcessMemory((int)processHandle, listStart + 4 * i, buffer, 4, ref bytesRead);
                    int objStart = Memory.ReadInt(processHandle, listStart + 4 * i, buffer);
                    if (objStart == 0)
                        continue;
                    int objClass = Memory.ReadInt(processHandle, objStart + 4, buffer);
                    string objClassName = Memory.ReadString(processHandle, objClass + 4, buffer);

                    if (objClassName != "obj_Levelsizer" &&
                        //objClassName != "obj_Turret" && duplicate of obj_ai_turret, but this one doesnt have valid armor,ad and other stats.
                        objClassName != "obj_AI_Turret" &&
                        objClassName != "AIHeroClient" &&
                        objClassName != "obj_AI_Minion")
                        continue;
                    Unit unit = Unit.GetUnit(processHandle, listStart, i);
                    localUnits.Add(unit);
                }
                lock (units)
                    units = localUnits; //everytime we get the new list, replace with original.
                Thread.Sleep(10);
            }
        }
        private static Thread loopThread = null;

        public static float Distance(Unit unit1, Unit unit2)
        {
            return (float)Math.Sqrt(Math.Pow((unit1.x - unit2.x), 2) + Math.Pow((unit1.y - unit2.y), 2));
        }

        public static void LookXY(float x, float y)
        {
            byte[] buffer = new byte[4];
            int cameraBase = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.Camera.baseAdress, buffer);
            cameraBase = Memory.ReadInt(processHandle, cameraBase + Offsets.Camera.Offset0, buffer);
            Memory.WriteFloat(processHandle, cameraBase + Offsets.Camera.X, x);
            Memory.WriteFloat(processHandle, cameraBase + Offsets.Camera.Y, y);
        }

        public static void LookAtTarget(Unit target)
        {
            LookXY(target.x, target.y);
        }
        /// <summary>
        /// sends a floating text over a target unit.
        /// please note that not all message types works on all objects,
        /// for instance on minions and jungle the messageTypes i found to work are 6,7,8 
        /// (didn't search beyond 10, so there might be more)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public static void FloatingText(Unit target, string message, uint messageType)
        {
            Communicator.GetInstance().LoLFloatingText(target.baseAddr, message, messageType);
        }

        public static List<Unit> GetAllObjects()
        {
            if (units == null)
                throw new Exception("error - unit list not initialized - have you called Init?");
            lock (units)
            {
                return new List<Unit>(units);
            }
        }
        public static List<Unit> GetAllChampions()
        {
            lock (units)
            {
                return units.Where(u => u is Player).ToList();
            }
        }
        public static List<Minion> GetAllMinions()
        {
            lock (units)
            {
                return units.Where(u => u is Minion).Cast<Minion>().ToList();
            }
        }
        public static Unit GetMyHero()
        {
            if (processHandle == null || moduleHandle == null)
                throw new Exception("not inited?");
            byte[] buffer = new byte[4];
            int heroObject = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.ObjectList.OurHero, buffer);
            string ourName = Memory.ReadString(processHandle, heroObject + 0x24, buffer);
            foreach (Unit u in GetAllObjects())
            {
                if (u.name == ourName)
                    return u;
            }
            return null;
        }
        public static int GetMyLevel()
        {
            byte[] buffer = new byte[4];
            int levelStructStart = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.Level.baseOffset, buffer);
            int add = Memory.ReadInt(processHandle, levelStructStart + Offsets.Level.offset0, buffer);
            int level = Memory.ReadInt(processHandle, add + Offsets.Level.level, buffer);
            return level;
        }
        public static bool IsVisible(Unit u)
        {
            byte[] buffer = new byte[4];
            int listBase = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.ObjectList.ListBegin, buffer);
            int unitStruct = Memory.ReadInt(processHandle, listBase + 4 * u.GetId(), buffer);
            int structVisible = Memory.ReadInt(processHandle, unitStruct + Offsets.Unit.unitVisibleStruct, buffer);
            byte visible = Memory.ReadByte(processHandle, structVisible + Offsets.Unit.unitVisibleStruct_unitVisible, buffer);
            return visible == 1;
        }
        public static void Init()
        {
            if (loopThread != null)
                throw new Exception("Already running!");
            loopThread = new Thread(Loop);
            loopThread.Start();
            units = new List<Unit>();
            Thread.Sleep(50);
        }
        public static void Stop()
        {
            if (loopThread != null && loopThread.IsAlive)
            {
                loopThread.Abort();
                loopThread = null;
            }
            units = null;
        }

        public static bool CanCastSpell(string spellLetter)
        {
            byte[] buffer = new byte[4];
            int levelStructStart = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.Level.baseOffset, buffer);
            int add = Memory.ReadInt(processHandle, levelStructStart + Offsets.Level.offset0, buffer);
            byte canCast = 255;
            int CanCastOffset = 0;
            spellLetter = spellLetter.ToLower();
            switch(spellLetter){
                case "d":
                    CanCastOffset = Offsets.Level.summonerSpell1OnCd;
                    break;
                case "f":
                    CanCastOffset = Offsets.Level.summonerSpell2OnCd;
                    break;
                case "q":
                    CanCastOffset = Offsets.Level.spellQOnCd;
                    break;
                case "w":
                    CanCastOffset = Offsets.Level.spellWOnCd;
                    break;
                case "e":
                    CanCastOffset = Offsets.Level.spellEOnCd;
                    break;
                case "r":
                    CanCastOffset = Offsets.Level.spellROnCd;
                    break;
            }
            if (CanCastOffset == 0)
                return false;
            canCast = Memory.ReadByte(processHandle, add + CanCastOffset, buffer);
            return canCast == 0; //0=not cd
        }
        public static Camera GetCamera()
        {
            byte[] buffer = new byte[4];
            int cameraBase = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.Camera.baseAdress, buffer);
            int cameraStruct = Memory.ReadInt(processHandle, cameraBase + Offsets.Camera.Offset0, buffer);
            return new Camera(){CameraX = Memory.ReadFloat(processHandle, cameraStruct + Offsets.Camera.X, buffer),
                                CameraY = Memory.ReadFloat(processHandle, cameraStruct + Offsets.Camera.Y, buffer),
                                CameraZ = Memory.ReadFloat(processHandle, cameraStruct + Offsets.Camera.Z, buffer),
                                CameraAngle = Memory.ReadFloat(processHandle, cameraStruct + Offsets.Camera.AngleLook, buffer),
                                CameraFovY = Memory.ReadFloat(processHandle, cameraStruct + Offsets.Camera.FovY, buffer),
            };
        }
        /// <summary>
        /// Get the point on the screen from the point in the world
        /// </summary>
        /// <param name="cameraX"></param>
        /// <param name="cameraY"></param>
        /// <param name="cameraZ"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <param name="cameraAngle">The angle between the ray of the camera and the ground</param>
        /// <param name="fovYAngle">The field of view angle of the camera in y axis</param>
        /// <returns></returns>
        private static Point WorldToScreen(float cameraX, float cameraY, float cameraZ, float targetX, float targetY, float targetZ, double cameraAngle, double fovYAngle)
        {
            cameraAngle = cameraAngle * Math.PI / 180;
            fovYAngle = fovYAngle * Math.PI / 180;
            //cameraZ -= targetZ;

            int screenWidth = Win32.GetSystemMetrics(0);
            int screenHeight = Win32.GetSystemMetrics(1);

            //double FovXAngle = 2 * Math.Atan(screenWidth * Math.Tan(fovYAngle / 2) / screenHeight);
            double FovXAngle = fovYAngle;
            fovYAngle = 2 * Math.Atan(screenHeight * Math.Tan(FovXAngle / 2) / screenWidth);

            double distanceFromScreen = (screenHeight / 2) / Math.Tan(fovYAngle / 2);

            double cameraBaseY = cameraY - cameraZ / Math.Tan(cameraAngle);
            double dy = targetY - cameraBaseY;
            //double angleToPoint = Math.Atan((cameraZ-targetZ) / dy);
            //double angleAtPov = 2 * Math.PI - angleToPoint - (Math.PI - cameraAngle);
            double angleAtPov = Math.Atan(dy / (cameraZ - targetZ)) - (Math.PI / 2 - cameraAngle);

            double screenY = -distanceFromScreen * Math.Tan(angleAtPov) + screenHeight / 2;

            double dx = targetX - cameraX;
            double cameraRayLength = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(cameraZ - targetZ, 2));
            double xAngleAtPov = Math.Atan(dx / cameraRayLength);

            double distanceFromScreenX = (screenWidth / 2) / Math.Tan(FovXAngle / 2);

            double screenX = distanceFromScreenX * Math.Tan(xAngleAtPov) + screenWidth / 2;

            return new Point((int)screenX, (int)screenY);
        }

        [Obsolete("should no longer be used")]
        public static Point WorldToScreen(float targetX, float targetY, float targetZ)
        {
            Camera c = GetCamera();
            return WorldToScreen(c.CameraX, c.CameraY, c.CameraZ, targetX, targetY, targetZ, c.CameraAngle, c.CameraFovY);
        }
        /// <summary>
        /// convert the world coords to screen coords.
        /// note: the z of the unit is the bottom of it, so i just added 60 to it
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [Obsolete("should no longer be used")]
        public static Point WorldToScreen(Unit unit)
        {
            Camera c = GetCamera();
            return WorldToScreen(c.CameraX, c.CameraY, c.CameraZ, unit.x, unit.y, unit.z + 60, c.CameraAngle, c.CameraFovY);
        }
        public static bool IsOnLoadingScreen()
        {
            List<Unit> units = GetAllObjects();
            foreach (Unit u in units)
            {
                if (u.className == "obj_Levelsizer")
                    return true;
            }
            return false;
        }
    }
}
