using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using lolcomjector;

namespace ObjectReader
{
    public struct Vector2
    {
        public int x;
        public int y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public enum MessageType : uint
    {
        White = 0,
        Yellow = 0x1,
        Green = 0x2,
        White_SlideLeft = 0x5,
        Red = 0x6,
        Purple = 0x7,
        Blue_Levelup = 0xB
    }
    public static class Engine
    {
        private static List<Unit> units = new List<Unit>();
        public static IntPtr processHandle { get; private set; }
        public static IntPtr moduleHandle { get; private set; }

        public static bool IsLolRunning { get { return Win32.FindWindow(null, "League of Legends (TM) Client") != IntPtr.Zero; } }

        private static void Loop()
        {
            byte[] buffer = new byte[4];

            while (!IsLolRunning)
            {
                Console.WriteLine("No league" + "," + DateTime.Now);
                System.Threading.Thread.Sleep(1000);
            }
            Process[] ps = Process.GetProcessesByName("League of Legends");
            Process p = ps[0];
            processHandle = Win32.OpenProcess((int)Win32.ProcessAccessFlags.All, false, p.Id);
            IntPtr ptr = Win32.CreateToolhelp32Snapshot(Win32.SnapshotFlags.Module, Convert.ToUInt32(p.Id));
            Win32.MODULEENTRY32 module = new Win32.MODULEENTRY32();
            module.dwSize = Convert.ToUInt32(Marshal.SizeOf(module));
            Win32.Module32First(ptr, ref module);
            while (module.szModule != "League of Legends.exe")
            {
                Win32.Module32Next(ptr, ref module);
            }
            moduleHandle = module.modBaseAddr;
            int objsNum = 0x9C00 / 4; //size of the obj list. not accurate, TODO: find the actual size in lol memory
            byte[] buff = new byte[4];
            int listStart = 0;
            while (listStart == 0)
            {
                listStart = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.ObjectList.ListBegin, buffer);
                Thread.Sleep(100);
            }
            while (IsLolRunning)
            {
                List<Unit> localUnits = new List<Unit>();
                for (int i = 0; i < objsNum; i++)
                {
                    Unit unit = Unit.GetUnit(processHandle, listStart, i);
                    if (unit != null)
                        localUnits.Add(unit);
                }
                lock (units)
                    units = localUnits; //everytime we get the new list, replace with original.
                Thread.Sleep(10);
            }
        }
        private static Thread loopThread = null;

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

        private static Dictionary<int, DateTime> lastWriteTime = new Dictionary<int, DateTime>();
        private const int WRITE_DELAY = 500; //TODO: make it so each module can write every WRITE_DELAY, instead of all of them can write every WRITE_DELAY
        /// <summary>
        /// sends a floating text over a target unit.
        /// please note that not all message types works on all objects,
        /// for instance on minions and jungle the messageTypes i found to work are 6,7,8 
        /// (didn't search beyond 10, so there might be more)
        /// 
        /// has a defence mechanism, not allowing text to be written on the same unit in a short delay
        /// (minimum time between calls 500 ms)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <returns>returns true if a floating text was sent.</returns>
        public static bool FloatingText(Unit target, string message, MessageType messageType = MessageType.Red) //TODO: this is bugged - sometimes won't write when it should
        {
            if (!target.IsVisible() || target.isDead) //don't send if the target is dead or not visible
                return false;
            if (!lastWriteTime.ContainsKey(target.GetId()))
                lastWriteTime.Add(target.GetId(), DateTime.Now);
            else
            {
                if ((DateTime.Now - lastWriteTime[target.GetId()]).TotalMilliseconds > WRITE_DELAY)
                    lastWriteTime[target.GetId()] = DateTime.Now;
                else
                    return false; //return false if not enough time has passed
            }
            Communicator.GetInstance().LolFloatingText(target.baseAddr, message, (uint)messageType);
            return true;
        }

        public static List<T> GetAll<T>()
        {
            return GetAllObjects().OfType<T>().ToList();
        }

        public static List<Unit> GetAllObjects()
        {
            lock (units)
            {
                return new List<Unit>(units);
            }
        }

        internal static int GetMyHeroId()
        {
            if (processHandle == null || moduleHandle == null)
                throw new Exception("not inited?");
            byte[] buffer = new byte[4];
            int heroObject = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.ObjectList.OurHero, buffer);
            string ourName = Memory.ReadString(processHandle, heroObject + Offsets.Unit.name, buffer);
            Champion mainChampion = GetAll<Champion>().Where(u => u.name == ourName).FirstOrDefault();
            if (mainChampion == null) //this will probably happen on first iteration
                return -1;
            return mainChampion.GetId();
        }
        
        public static void Init()
        {
            if (loopThread != null)
                throw new Exception("Already running!");
            loopThread = new Thread(Loop);
            loopThread.Start();
            units = new List<Unit>();
            processHandle = IntPtr.Zero;
            moduleHandle = IntPtr.Zero;
            Thread.Sleep(50);
        }
        public static void Stop()
        {
            if (loopThread != null && loopThread.IsAlive)
            {
                loopThread.Abort();
                loopThread = null;
            }
            units = new List<Unit>();
        }

        public static void MoveTo(float x, float z, float y)
        {
            Communicator.GetInstance().LolMoveTo(x, z, y, MoveType.Move, Champion.Me.baseAddr, 0);
        }

        public static void MoveTo(Unit u)
        {
            Communicator.GetInstance().LolMoveTo(0, 0, 0, MoveType.Move, Champion.Me.baseAddr, u.baseAddr);
        }

        /// <summary>
        /// acts like pressing s in game
        /// </summary>
        public static void StopMove()
        {
            Communicator.GetInstance().LolMoveTo(0, 0, 0, MoveType.Stop, Champion.Me.baseAddr, 0);
        }

        public static void Attack(Unit u)
        {
            Communicator.GetInstance().LolMoveTo(0, 0, 0, MoveType.Attack, Champion.Me.baseAddr, u.baseAddr);
        }

        public static void AttackMoveTo(Unit u)
        {
            Communicator.GetInstance().LolMoveTo(0, 0, 0, MoveType.AttackMove, Champion.Me.baseAddr, u.baseAddr);
        }

        public static void AttackMoveTo(float x, float y, float z)
        {
            Communicator.GetInstance().LolMoveTo(x, y, z, MoveType.AttackMove, Champion.Me.baseAddr, 0);
        }

        public static bool CanCastSpell(string spellLetter)
        {
            byte[] buffer = new byte[4];
            int levelStructStart = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.Level.baseOffset, buffer);
            int add = Memory.ReadInt(processHandle, levelStructStart + Offsets.Level.offset0, buffer);
            byte canCast = 255;
            int CanCastOffset = 0;
            spellLetter = spellLetter.ToLower();
            switch (spellLetter)
            {
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
            return canCast == 0; //0=not on cd
        }
        public static Camera GetCamera()
        {
            byte[] buffer = new byte[4];
            int cameraBase = Memory.ReadInt(processHandle, (int)moduleHandle + Offsets.Camera.baseAdress, buffer);
            int cameraStruct = Memory.ReadInt(processHandle, cameraBase + Offsets.Camera.Offset0, buffer);
            return new Camera()
            {
                CameraX = Memory.ReadFloat(processHandle, cameraStruct + Offsets.Camera.X, buffer),
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
        private static Vector2 WorldToScreen(float cameraX, float cameraY, float cameraZ, float targetX, float targetY, float targetZ, double cameraAngle, double fovYAngle)
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

            return new Vector2((int)screenX, (int)screenY);
        }

        public static Vector2 WorldToScreen(float targetX, float targetY, float targetZ)
        {
            Camera c = GetCamera();
            return WorldToScreen(c.CameraX, c.CameraY, c.CameraZ, targetX, targetY, targetZ, c.CameraAngle, c.CameraFovY * 1.4);
        }
        /// <summary>
        /// convert the world coords to screen coords.
        /// note: the z of the unit is the bottom of it, so i just added 60 to it
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Vector2 WorldToScreen(Unit unit)
        {
            Camera c = GetCamera();
            return WorldToScreen(c.CameraX, c.CameraY, c.CameraZ, unit.x, unit.y, unit.z + 60, c.CameraAngle, c.CameraFovY * 1.4);
        }
        [Obsolete("this function must be optimized, currently not working",true)]
        public static bool IsOnLoadingScreen()
        {
            List<Unit> units = GetAllObjects();
            foreach (Unit u in units)
            {
                //TODO: make a class for obj_Levelsizer
                if (u.className == "obj_Levelsizer") //this object apears on loading screen
                    return true;
            }
            return false;
        }
    }
}
