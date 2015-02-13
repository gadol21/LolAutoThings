using lolcomjector;
using ObjectReader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LolThingies
{
    class WardRevealer : Module
    {
        private Thread thread;
        public WardRevealer(Keys key, int x, int y)
            : base(key, x, y)
        {

        }

        public override void Init()
        {
            Console.WriteLine("Started ward revieler");

        }
        public override void Start()
        {
            thread = new Thread(WardRevealerFunc);
            thread.Start();

            Console.WriteLine("ward revealer state changed on");
        }
        public override void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
            foreach (Ward w in Engine.GetAll<Ward>())
                Communicator.GetInstance().RemoveText(w.type + " ward here");
            Console.WriteLine("ward revealer state changed off");
        }

        public void WardRevealerFunc()
        {
            while (true)
            {
                Champion myPlayer = Engine.GetMyHero();
                foreach (Ward ward in Engine.GetAll<Ward>().Where(w => w.team == myPlayer.team)) //remove the texts from their old location
                    Communicator.GetInstance().RemoveText(ward.type + " ward here");
                foreach (Ward ward in Engine.GetAll<Ward>().Where(w => w.team == myPlayer.team)) //get all wards of the enemy team
                {
                    if (ward.isDead)
                        continue;
                    Point p = Engine.WorldToScreen(ward);
                    Communicator.GetInstance().SendText(ward.type+" ward here",100 , 20, p.X, p.Y,TextFormat.Center);
                }
                Thread.Sleep(20);
            }
        }
    }
}
