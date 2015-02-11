using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lolcomjector;

namespace LolThingies
{
    abstract class Module
    {
        private int x, y;
        public Keys Key { get; set; }
        protected bool on = false;
        private int fontSize = 20;
        private Argb color;
        private string displayText = "";
        protected readonly string name;
        public Module(Keys key, int x,int y)
        {
            name = this.GetType().Name;
            //AutoLaugh => Auto Laugh
            for (int i = 1; i < name.Length; i++)
            {
                if (Char.IsUpper(name[i]))
                {
                    name = name.Insert(i, " ");
                    i += 1;
                }
            }
            displayText = name +"("+Enum.GetName(typeof(Keys),key)+"): " + ((on) ? "On" : "Off");
            on = false;
            this.x = x;
            this.y = y;
            color = new Argb(255, 0, 255, 255);
            this.Key = key;
        }
        public virtual void Init()
        {
            Communicator.GetInstance().SendTextUnlimitedTime(displayText, fontSize, x, y, color);
        }
        public virtual void KeyPress()
        {
            on = !on;
            Communicator.GetInstance().RemoveText(displayText);
            displayText = name + "(" + Enum.GetName(typeof(Keys), Key) + "): " + ((on) ? "On" : "Off");
            Communicator.GetInstance().SendTextUnlimitedTime(displayText, fontSize, x, y, color);
        }
        public virtual void Stop()
        {

        }
    }
}
