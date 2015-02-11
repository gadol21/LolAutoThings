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
                foreach (Minion minion in LoLReader.GetAll<Minion>())
                {
                    //Console.WriteLine(unit.name);
                    if (minion.name.StartsWith("SRU") && !minion.name.Contains("Mini") && !minion.name.Contains("Wall")) //temporary workaround for the new summoners rift
                    {
                        if(LoLReader.IsVisible(minion))
                        {
                            if (minion.hp > 0 && !minion.isDead)
                            {
                                Unit myHero = LoLReader.GetMyHero();
                                if (myHero == null)
                                    break;
                                if (minion.hp < minion.maxhp && LoLReader.Distance(minion, myHero) < SMITE_RANGE && LoLReader.CanCastSpell(key))
                                { //floating text
                                    if (!lastWriteTime.ContainsKey(minion.GetId()))
                                        lastWriteTime.Add(minion.GetId(), DateTime.Now);
                                    if ((DateTime.Now - lastWriteTime[minion.GetId()]).Milliseconds > 600)//make a delay between calls
                                    {
                                        LoLReader.FloatingText(minion, "Ready To Smite", MessageType.red);
                                        lastWriteTime[minion.GetId()] = DateTime.Now;
                                    }
                                }
                                int myLevel = LoLReader.GetMyLevel();
                                if (myLevel < 1 || myLevel > 18) // happens sometimes when the game ends
                                    break;
                                if (minion.hp <= smiteDmg[myLevel - 1])
                                {
                                    //check distance
                                    if (LoLReader.Distance(minion, myHero) < SMITE_RANGE)
                                    {
                                        if (LoLReader.CanCastSpell(key))
                                        {
                                            //maybe install a mouse hook to disable mouse movement while moving mouse and smiting
                                            Console.WriteLine("smiting " + minion.name + DateTime.Now);
                                            //move camera
                                            LoLReader.LookAtTarget(minion);
                                            //press smite button

                                            //unit.z += 60;

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
