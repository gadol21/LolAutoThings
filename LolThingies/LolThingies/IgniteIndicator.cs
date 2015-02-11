using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ObjectReader;
using lolcomjector;
using System.Drawing;
using System.Windows.Forms;

namespace LolThingies
{
    class IgniteIndicator : Module
    {
        private readonly int[] igniteDps = { 14, 18, 22, 26, 30, 34, 38, 42, 46, 50, 54, 58, 62, 66, 70, 74, 78, 82 };
        private Thread thread;

        public IgniteIndicator(Keys key, int x, int y)
            : base(key, x, y)
        {
        }

        public override void Init()
        {
            Console.WriteLine("Started Ignite Indicator");
        }
        public void IgniteLoop()
        {
            Dictionary<int, DateTime> lastWriteTime = new Dictionary<int, DateTime>();
            while (true)
            {
                Unit me = LoLReader.GetMyHero();
                foreach (Unit u in LoLReader.GetAllChampions())
                {
                    if (me == null)
                        break;
                    if (u.team == me.team)
                        continue;
                    if (u.hp <= 0 || !LoLReader.IsVisible(u))
                        continue;
                    int myLevel = LoLReader.GetMyLevel();
                    if (myLevel <= 0) //happens when the game ends?
                        continue;
                    if (u.hp < igniteDps[myLevel - 1]*5) 
                    {
                        bool kill = false;
                        float hp = u.hp;
                        for (int i = 0; i < 5; i++) //apply 5 ticks of ignite, consider hp regen, and see if the target unit dies.
                        {
                            hp += u.hpRegenPerSec;
                            hp -= igniteDps[myLevel - 1];
                            if (hp <= 0)
                            {
                                kill = true;
                                break;
                            }
                        }
                        if (kill) //ignite will kill
                        {
                            //Point p = LoLReader.WorldToScreen(u);
                            bool firstTime = false;
                            if (!lastWriteTime.ContainsKey(u.GetId()))
                            {
                                lastWriteTime.Add(u.GetId(), DateTime.Now);
                                firstTime = true;
                            }
                            if (firstTime || (DateTime.Now - lastWriteTime[u.GetId()]).Milliseconds > 400) //floating text every 400 sec?
                            {
                                Console.WriteLine("ignite will kill " + u.name);
                                LoLReader.FloatingText(u, "ignite! ", MessageType.red);
                                lastWriteTime[u.GetId()] = DateTime.Now;
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
        public override void KeyPress()
        {
            if (on)
            {
                thread = new Thread(IgniteLoop);
                thread.Start();
            }
            else
            {
                if (thread != null && thread.IsAlive)
                {
                    thread.Abort();
                   // for (int i = 0; i < 5; i++)
                    //    Communicator.GetInstance().RemoveText("Ignite WILL KILL");
                }
            }
        }
        public override void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                //for (int i = 0; i < 5; i++)
                 //   Communicator.GetInstance().RemoveText("Ignite WILL KILL");
            }
        }
    }
}
