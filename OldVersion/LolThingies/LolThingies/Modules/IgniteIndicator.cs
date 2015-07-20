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
            while (true)
            {
                MainChampion me = Champion.Me;
                foreach (Unit u in Engine.GetAll<Champion>())
                {
                    if (me == null)
                        break;
                    if (u.team == me.team)
                        continue;
                    if (u.hp <= 0 || u.isDead || !u.IsVisible())
                        continue;
                    int myLevel = me.level;
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
                            Console.WriteLine("ignite will kill " + u.name);
                            Engine.FloatingText(u, "ignite! ", MessageType.Red);
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
        public override void Start()
        {
                thread = new Thread(IgniteLoop);
                thread.Start();
        }
        public override void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }
}
