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
    class WardRevieler : Module
    {
        private Thread thread;
        public WardRevieler(Keys key, int x, int y)
            : base(key, x, y)
        {

        }

        public override void Init()
        {
            Console.WriteLine("Started ward revieler");

        }
        public override void KeyPress()
        {
            if (on)
            {
                thread = new Thread(WardRevielerFunc);
                thread.Start();
            }
            else
            {
                thread.Abort();
            }

            Console.WriteLine("ward revieler state changed " + on);
        }
        public override void Stop()
        {
            bool statment = thread != null && thread.IsAlive;
            if (statment)
            {
                thread.Abort();
            }
            foreach (Ward w in LoLReader.GetAll<Ward>())
                Communicator.GetInstance().RemoveText(w.type + " ward here");
        }

        public void WardRevielerFunc()
        {
            while (true)
            {
                Champion myPlayer = LoLReader.GetMyHero();
                foreach (Ward ward in LoLReader.GetAll<Ward>().Where(w => w.team != myPlayer.team))
                    Communicator.GetInstance().RemoveText(ward.type + " ward here");
                foreach (Ward ward in LoLReader.GetAll<Ward>().Where(w => w.team != myPlayer.team)) //get all wards of the enemy team
                {
                    if (ward.isDead)
                        continue;
                    Point p = LoLReader.WorldToScreen(ward);
                    Communicator.GetInstance().SendText(ward.type+" ward here",100 , 20, p.X, p.Y,TextFormat.Center);
                }
                Thread.Sleep(20);
            }
        }
    }
}
