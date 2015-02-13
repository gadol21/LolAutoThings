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
    class CloneDetector : Module
    {
        private Thread thread;
        public CloneDetector(Keys key, int x, int y)
            : base(key, x, y)
        {

        }

        public override void Init()
        {
            Console.WriteLine("Started clone detector");

        }
        public override void Start()
        {
            thread = new Thread(CloneDetectorFunc);
            thread.Start();

            Console.WriteLine("clone detector state changed on");
        }
        public override void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
            Console.WriteLine("clone detector state changed off");
        }

        public void CloneDetectorFunc()
        {
            while (true)
            {
                //dictionary of champion names, and how many times they apear.
                //clones apear with the same name of the real champions, but as minions.
                //just check how many names apear twice, and than write on the player that he is the real one
                Dictionary<string, int> champions = new Dictionary<string, int>();
                foreach (Unit u in Engine.GetAllObjects())
                {
                    if (u is Minion || u is Champion)
                    {
                        if (!champions.ContainsKey(u.name))
                            champions.Add(u.name, 1);
                        else
                            champions[u.name] += 1;
                    }
                }
                foreach (var pair in champions.Where(pair => pair.Value == 2)) //find all champions who apear twice
                {
                    foreach (Unit u in Engine.GetAllObjects().Where(u => u.name == pair.Key))
                    {
                        if (u is Champion)
                            Engine.FloatingText(u, "real one", MessageType.Red);
                    }
                }
                Thread.Sleep(400);
            }
        }
    }
}
