using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AutoItX3Lib;
using lolcomjector;
using ObjectReader;

namespace LolThingies
{
    class AutoLaugh:Module
    {  

        private Thread thread;
        public AutoLaugh(Keys key,int x,int y):base(key,x,y)
        {
           
        }
        public override void Init()
        {
            Console.WriteLine("Started auto laugh");
        }
        public override void Start()
        {
            thread = new Thread(AutoLaughFunc);
            thread.Start();
            
            Console.WriteLine("Auto laugh state changed on");
        }
        public override void Stop()
        {
            if (thread!=null && thread.IsAlive)
                thread.Abort();
            AutoItX3Declarations.AU3_Send("{4 UP}{CTRLUP}", 0);
            Console.WriteLine("Auto laugh state changed off");
        }
        public void AutoLaughFunc()
        {
            while (true)
            {
                IntPtr hWnd = Win32.FindWindow(null, "League of Legends (TM) Client");
                if (Engine.IsLolRunning)
                {
                    if (Win32.GetForegroundWindow() == hWnd)
                    {
                        AutoItX3Declarations.AU3_Send("{CTRLDOWN}4{CTRLUP}", 0);
                        Console.WriteLine("Laugh");
                    }
                }
                else
                {
                    Stop();
                    return;
                }
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
