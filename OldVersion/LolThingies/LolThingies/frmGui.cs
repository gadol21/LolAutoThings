﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using lolcomjector;
using ObjectReader;

namespace LolThingies
{
    public partial class frmGUI : Form
    {
        #region DLLS
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion

        #region Hook
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x105;
        private const int HC_ACTION = 0;

        private const int VK_F8 = 0x77;
        private const int VK_F10 = 0x79;

        private IntPtr hookPtr = IntPtr.Zero; //hook things

        private const string dllName = "d3dPresentHook.dll";
        private List<Module> modules;

        private Thread injectingWaitingThread;
        private Thread waitingForGameToEndThread;

        private keyboardHookProc myDelegate;

        public frmGUI()
        {
            AutoItX3Declarations.AU3_Opt("SendKeyDownDelay", 100);

            myDelegate = hookFunction;

            InitializeComponent();
            rdbInjectManually.Checked = false;
            btnInject.Enabled = false;

            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "League of Legends")
                {
                    if (Injector.AlreadyInjected(p, Directory.GetCurrentDirectory() + "\\" + dllName))
                            new Thread(delegate()
                            {
                                Thread.Sleep(100);
                                AfterInject();
                            }).Start();
                    break;
                }
            }

            modules = new List<Module>();
            modules.Add(new AutoLaugh(Keys.F9, 5, 5));
            modules.Add(new AutoSmite(Keys.F8, 5, 25,rdbSmiteF.Checked ? "F" : "D"));
            modules.Add(new IgniteIndicator(Keys.F7, 5, 45));
            modules.Add(new WardRevealer(Keys.F6, 5, 65));
            modules.Add(new CloneDetector(Keys.F5, 5, 85));

            hookPtr = SetWindowsHookEx(13, myDelegate, IntPtr.Zero, 0); //WH_KEYBOARD_LL=13
            if (hookPtr == IntPtr.Zero)
            {
                MessageBox.Show("Failed to hook");
                Close();
            }
            Console.WriteLine("keyboard hook installed"); //im a hooker yo

        }

        private void SetInjectedGui()
        {
            rdbInjcetAuto.Hide();
            rdbInjectManually.Hide();
            btnInject.Hide();
            btnStart.Hide();
            this.Invoke(new Action(()=>lblStatus.Text = "Status: Injected"));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (Module f in modules)
            {
                f.Stop();
            }
            UnhookWindowsHookEx(hookPtr);
            Communicator.GetInstance().Clear();
            Engine.Stop();
            base.OnClosing(e);
            Environment.Exit(0);
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            Inject();   
        }

        private void Inject()
        {
            try
            {
                Injector.Inject("League of Legends", Directory.GetCurrentDirectory() + "\\" + dllName);
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                if (ex is InvalidProcess)
                {
                    MessageBox.Show("LoL not running");
                }
                else if (ex is InvalidDllPath)
                {
                    MessageBox.Show("Wrong Dll Path");
                }
                else if (ex is AlreadyInjected)
                {
                    MessageBox.Show("Already injected to LoL!");
                }
                else
                    MessageBox.Show("Unkown excaption:" + ex.Message);
            }
            AfterInject();
        }
        private void AfterInject()
        {
            Communicator.GetInstance().Clear();
            Communicator.GetInstance().SendText("Injected Successfuly", 3000, 20, 50, 100);
            Engine.Init();
            foreach (Module f in modules)
            {
                f.OnInit();
            }
            this.Invoke(new Action(() => SetInjectedGui()));
            waitingForGameToEndThread = new Thread(WaitingForGameToEndFunc);
            waitingForGameToEndThread.Start();
        }

        private int hookFunction(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (code == HC_ACTION && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
            {
                foreach (Module f in modules)
                {
                    if (lParam.vkCode == (int)f.Key)
                    {
                        f.OnKeyPress();
                        break;
                    }
                }
                //Console.WriteLine(code + "," + lParam.vkCode + "," + wParam);
            }
            return CallNextHookEx(hookPtr, code, wParam, ref lParam);
        }

        private void rdbInjcetAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbInjcetAuto.Checked)
            {
                btnStart.Show();
            }
            else
            {
                btnStart.Hide();
                if(injectingWaitingThread!=null && injectingWaitingThread.ThreadState== System.Threading.ThreadState.Running)
                    injectingWaitingThread.Abort();
            }
        }

        private void rdbInjectManually_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbInjectManually.Checked)
            {
                btnInject.Enabled = true;
                lblStatus.Text = "Status: Not Injected";
            }
            else
            {
                btnInject.Enabled = false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            injectingWaitingThread = new Thread(injectionWaitingFunction);
            injectingWaitingThread.Start();
            btnStart.Hide();
            lblStatus.Text = "Status: Waiting for lol";
        }

        private void injectionWaitingFunction()
        {
            while (!Engine.IsLolRunning)
            {
                Thread.Sleep(250);
            }
            //league is running
            Inject();
        }
        private void WaitingForGameToEndFunc()
        {
            while (Engine.IsLolRunning)
            {
                Thread.Sleep(1000);
            }
            foreach (Module f in modules)
            {
                f.Stop();
            }
            Engine.Stop();
            this.Invoke(new Action(()=>SetRegularGui()));
        }
        private void SetRegularGui()
        {
            rdbInjcetAuto.Show();
            rdbInjectManually.Show();
            btnInject.Show();
            btnStart.Show();
            //this.Invoke(new Action(() => lblStatus.Text = "Status: Injected")); why is this here?
            lblStatus.Text = "Status: Not Injected";
            rdbInjcetAuto_CheckedChanged(null, null);
            rdbInjectManually_CheckedChanged(null, null);
            if (rdbInjcetAuto.Checked)
            {
                btnStart_Click(null,null);
            }
        }

        private void rdbSmiteD_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Module f in modules)
            {
                if (f is AutoSmite)
                {
                    ((AutoSmite)f).SetSmiteKey("d");
                }
            }
        }

        private void rdbSmiteF_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Module f in modules)
            {
                if (f is AutoSmite)
                {
                    ((AutoSmite)f).SetSmiteKey("f");
                }
            }
        }
    }
}
