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
        public override void KeyPress()
        {
            if (on)
            {
                thread = new Thread(CloneDetectorFunc);
                thread.Start();
            }
            else
            {
                thread.Abort();
            }

            Console.WriteLine("clone detector state changed " + on);
        }
        public override void Stop()
        {
            bool statment = thread != null && thread.IsAlive;
            if (statment)
            {
                thread.Abort();
            }
        }

        public void CloneDetectorFunc()
        {
            while (true)
            {
                Dictionary<string, int> champions = new Dictionary<string, int>();
                foreach (Unit u in LoLReader.GetAllObjects())
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
                    foreach (Unit u in LoLReader.GetAllObjects().Where(u => u.name == pair.Key))
                    {
                        if (u is Champion)
                            LoLReader.FloatingText(u, "real one", MessageType.red);
                    }
                }
                Thread.Sleep(400);
            }
        }
    }
}
