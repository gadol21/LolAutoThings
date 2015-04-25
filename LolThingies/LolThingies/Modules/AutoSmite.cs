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
    class AutoSmite : Module
    {
        private readonly int[] smiteDmg = { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
        private const int SMITE_RANGE = 760;

        private Thread thread;
        private string smiteKey;
        public AutoSmite(Keys key, int x, int y, string smiteKey)
            : base(key, x, y)
        {
            this.smiteKey = smiteKey;
        }
        public override void Init()
        {
            Console.WriteLine("Started auto smite");

        }
        public override void Start()
        {
            thread = new Thread(AutoSmiteFunc);
            thread.Start();

            Console.WriteLine("Auto smite state changed on");
        }
        public override void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
            Console.WriteLine("Auto smite state changed off");
        }

        public void SetSmiteKey(string key)
        {
            if (key != "d" && key != "f")
                throw new Exception("Wrong key entered. valid key options: d, f");
            this.smiteKey = key;
        }

        public void AutoSmiteFunc()
        {
            while (true)
            {
                foreach (Minion minion in Engine.GetAll<Minion>())
                {
                    //Console.WriteLine(unit.name);
                    if (minion.name.StartsWith("SRU") && !minion.name.Contains("Mini") && !minion.name.Contains("Wall")) //temporary workaround for the new summoners rift
                    {
                        if (!minion.IsVisible()) //must be visible to smite
                            continue;
                        if (minion.hp > 0 && !minion.isDead)
                        {
                            Champion myHero = Engine.GetMyHero();
                            if (myHero == null)
                                break;
                            if (minion.hp < minion.maxhp && myHero.DistanceFrom(minion) < SMITE_RANGE && Engine.CanCastSpell(smiteKey))
                                Engine.FloatingText(minion, "Ready To Smite", MessageType.Red);
                            int myLevel = myHero.level;
                            if (myLevel < 1 || myLevel > 18) // happens sometimes when the game ends
                                break;
                            if (minion.hp <= smiteDmg[myLevel - 1])
                            {
                                //check distance
                                if (myHero.DistanceFrom(minion) < SMITE_RANGE && Engine.CanCastSpell(smiteKey))
                                {
                                    //maybe install a mouse hook to disable mouse movement while moving mouse and smiting
                                    Console.WriteLine("smiting " + minion.name + DateTime.Now);
                                    //move camera
                                    Engine.LookAtTarget(minion);
                                    AutoItX3Declarations.AU3_MouseMove(Win32.GetSystemMetrics(0) / 2, Win32.GetSystemMetrics(1) / 2, 0);
                                    AutoItX3Declarations.AU3_Send(smiteKey, 0);
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
