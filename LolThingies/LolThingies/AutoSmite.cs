using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using lolcomjector;
using System.Windows.Forms;
using ObjectReader;
using System.Drawing;

namespace LolThingies
{
    class AutoSmite:Module
    {
        private readonly int[] smiteDmg = { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
        private const int SMITE_RANGE = 760;

        private Dictionary<string, int> targets;
        private Thread thread;
        private string key;
        public AutoSmite(Keys key, int x,int y, string smiteKey):base (key,x,y)
        {
            this.key = smiteKey;
        }
        public override void Init()
        {
            base.Init();
            Console.WriteLine("Started auto smite");
            targets = new Dictionary<string, int>();
            targets.Add("dragon", 0);
            targets.Add("baron", 0);
            targets.Add("1 blue", 0);
            targets.Add("1 red", 0);
            targets.Add("2 blue", 0);
            targets.Add("2 red", 0);
            
        }
        public override void KeyPress()
        {
            base.KeyPress();
            if (on)
            {
                thread = new Thread(AutoSmiteFunc);
                thread.Start();
            }
            else
            {
                thread.Abort();
            }

            Console.WriteLine("Auto smite state changed " + on);
        }
        public override void Stop()
        {
            base.Stop();
            bool statment = thread != null && thread.IsAlive;
            if (statment)
            {
                thread.Abort();
            }
        }

        public void SetSmiteKey(string key)
        {
            if (key != "d" && key != "f")
                throw new Exception("Wrong key entered. valid key options: d, f");
            this.key = key;
        }

        public void AutoSmiteFunc()
        {
            Dictionary<int, DateTime> lastWriteTime = new Dictionary<int, DateTime>();
            while (true)
            {
                List<Unit> objs = LoLReader.GetAllObjects();
                foreach (Unit unit in objs)
                {
                    Console.WriteLine(unit.name);
                    if (unit.name.StartsWith("SRU") && !unit.name.Contains("Mini")) //temporary workaround for the new summoners rift
                    {
                        if(LoLReader.IsVisible(unit))
                        {
                            if (unit.hp > 0)
                            {
                                Unit myHero = LoLReader.GetMyHero();
                                if (unit.hp < unit.maxhp && LoLReader.Distance(unit, myHero) < SMITE_RANGE && LoLReader.CanCastSpell(key))
                                { //floating text
                                    if (!lastWriteTime.ContainsKey(unit.GetId()))
                                        lastWriteTime.Add(unit.GetId(), DateTime.Now);
                                    if ((DateTime.Now - lastWriteTime[unit.GetId()]).Milliseconds > 600)//make a delay between calls
                                    {
                                        LoLReader.FloatingText(unit, "Ready To Smite", 0x6);
                                        lastWriteTime[unit.GetId()] = DateTime.Now;
                                    }
                                }
                                int myLevel = LoLReader.GetMyLevel();
                                if (myLevel < 1 || myLevel > 18) // happens sometimes when the game ends
                                    break;
                                if (unit.hp <= smiteDmg[myLevel - 1])
                                {
                                    //check distance
                                    if (LoLReader.Distance(unit, myHero) < SMITE_RANGE)
                                    {
                                        if (LoLReader.CanCastSpell(key))
                                        {
                                            //maybe install a mouse hook to disable mouse movement while moving mouse and smiting
                                            Console.WriteLine("smiting " + unit.name + DateTime.Now);
                                            //move camera
                                            LoLReader.LookAtTarget(unit);
                                            //press smite button
                                            unit.z += 60;
                                            //Point p = LoLReader.WorldToScreen(unit);
                                            //Win32.RECT rect;
                                            /*Win32.GetWindowRect(Win32.FindWindow(null, "League of Legends (TM) Client"), out rect);
                                            if (rect.X < p.X && rect.X + rect.Width > p.X
                                                && rect.Y < p.Y && rect.Y + rect.Height > p.Y)
                                            {*/
                                                //AutoItX3Declarations.AU3_MouseMove(p.X, p.Y, 0); //maybe SetCurserPos instead
                                            AutoItX3Declarations.AU3_MouseMove(Win32.GetSystemMetrics(0) / 2, Win32.GetSystemMetrics(1) / 2, 0);
                                            AutoItX3Declarations.AU3_Send(key, 0);
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(20);
            }
        }
    }
}
