using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ObjectReader;
using lolcomjector;
using System.Drawing;

namespace LolThingies
{
    [Obsolete("Not needed any more, as there is FloatingText now")]
    class WriteOnMonster:Module
    {
        private Thread thread;

        private string[] strings;

        public WriteOnMonster(Keys key, int x, int y)
            : base(key, x, y)
        {
            strings = new string[5];
        }
        public override void Init()
        {
            Console.WriteLine("Started writing thing");
            Camera c = LoLReader.GetCamera();
            Point p = LoLReader.WorldToScreen(c.CameraX, c.CameraY,0);
            c.CameraFovY = 80;
            Console.WriteLine(c);
            Console.WriteLine("X:{0} Y:{1}",p.X,p.Y);
            
        }
        public override void KeyPress()
        {
            if (on)
            {
                thread = new Thread(WritingFunc);
                thread.Start();
            }
            else
            {
                thread.Abort();
                for (int i = 0; i < strings.Length; i++)
			    {
			        Communicator.GetInstance().RemoveText(strings[i]);
                }
            }
            Console.WriteLine("writing state changed " + on);
        }
        public override void Stop()
        {
            bool statment = thread != null && thread.IsAlive;
            if (statment)
            {
                thread.Abort();
            }
        }
        public void WritingFunc()
        {
            while (true)
            {
                List<Unit> objs = LoLReader.GetAllObjects();
                foreach (Unit unit in objs)
                {
                    string key = "";
                    if (unit.name.StartsWith("AncientGolem"))
                    {
                        if (unit.name.StartsWith("AncientGolem1"))
                            //blue team
                            key = "1 red";
                        else
                            key = "2 red";
                    }
                    if (key != "")
                    {
                        for (int i = 0; i < strings.Length; i++)
			            {
			                Communicator.GetInstance().RemoveText(strings[i]);
			            }
                        strings[0] = "Target pos x: "+unit.x;
                        strings[1] = "Target pos y: "+unit.y;
                        Point p = LoLReader.WorldToScreen(unit);
                        strings[2] = "Drawing pos x:"+p.X;
                        strings[3] = "Drawing pos y:"+p.Y;
                        strings[4] = "TARGET";
                        for (int i = 0; i < strings.Length-1; i++)
			                Communicator.GetInstance().SendTextUnlimitedTime(strings[i],20,5,60+20*i);
			            Communicator.GetInstance().SendTextUnlimitedTime(strings[strings.Length-1],20,p.X,p.Y,TextFormat.Center);
                    }
                }
                System.Threading.Thread.Sleep(5);
            }
        }
        
    }
}

