using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using lolcomjector;
using System.Threading;
using System.Runtime.InteropServices;
using Awesomium.Core;
using Awesomium.Windows.Forms;

namespace MusicForm
{    
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); 

        private const string dllName = "d3dPresentHook.dll";
        private int songY = 50,artistY = 100;
        private int x = 700;
        private string currentSong, currentArtist;
        private Thread keyboard;
        bool finishedLoading = false;
        private bool resendSong = false, resendArtist = false;

        private string CurrentSong
        {
            get { return currentSong; }
            set
            {
                if (resendSong || value != currentSong)//if the value is already ok, no need to change it and resend it
                {
                    if (currentSong != "" && currentSong != null)
                        Communicator.GetInstance().RemoveText(currentSong);
                    currentSong = value;
                    if (currentSong != "" && currentSong != null)
                        Communicator.GetInstance().SendTextUnlimitedTime(currentSong, 22, x, songY, new Argb(150, 220, 220, 0));
                    resendSong = false;
                }
            }

        }
        private string CurrentArtist
        {
            get { return currentArtist; }
            set
            {
                if (resendArtist || value != currentArtist)//if the value is already ok, no need to change it and resend it
                {
                    if (currentSong != "" && currentSong != null)
                        Communicator.GetInstance().RemoveText(currentArtist);
                    currentArtist = value;
                    if (currentSong != "" && currentSong != null)
                        Communicator.GetInstance().SendTextUnlimitedTime(currentArtist, 22, x, artistY, new Argb(150, 0, 220, 220));
                    resendArtist = false;
                }
            }

        }

        public Form1()
        {
            InitializeComponent();
        }

        void keyboardThread()
        {
            while (true)
            {
                if (!finishedLoading)
                {
                    Thread.Sleep(200);
                    continue;
                }
                if ((GetAsyncKeyState(Keys.F6) & 1) == 1) //the key was pressed since last check (play/pause)
                {
                    webControl.Invoke(new Action(() => //for some stupid reason, a control can only be accessed from its original thread
                        {
                            webControl.ExecuteJavascript("jwplayer().pause();");
                        }));
                }
                if ((GetAsyncKeyState(Keys.F3) & 1) == 1) //volume up
                {
                    webControl.Invoke(new Action(() =>
                        {
                            int volume = int.Parse(webControl.ExecuteJavascriptWithResult("jwplayer().getVolume()"));
                            volume += 2;
                            if (volume > 100)
                                volume = 100;
                            webControl.ExecuteJavascript("jwplayer().setVolume(" + volume + ")");
                        }));
                }
                if ((GetAsyncKeyState(Keys.F2) & 1) == 1) //volume down
                {
                    webControl.Invoke(new Action(() =>
                        {
                            int volume = int.Parse(webControl.ExecuteJavascriptWithResult("jwplayer().getVolume()"));
                            volume -= 2;
                            if (volume < 0)
                                volume = 0;
                            webControl.ExecuteJavascript("jwplayer().setVolume(" + volume + ")");
                        }));
                }
                if ((GetAsyncKeyState(Keys.F1) & 1) == 1) //mute
                {
                    webControl.Invoke(new Action(() =>
                        {
                            webControl.ExecuteJavascript("jwplayer().setMute()");
                        }));
                }
                if ((GetAsyncKeyState(Keys.F5) & 1) == 1) //prev song
                {
                    webControl.Invoke(new Action(() =>
                        {
                            int index = int.Parse(webControl.ExecuteJavascriptWithResult("jwplayer().getPlaylistItem().index"));
                            index -= 1;
                            if (index < 0)
                                index = 0;
                            webControl.ExecuteJavascript("jwplayer().playlistItem(" + index + ")");
                        }));
                    
                }
                if ((GetAsyncKeyState(Keys.F7) & 1) == 1) //next song
                {
                    webControl.Invoke(new Action(() =>
                        {
                            int index = int.Parse(webControl.ExecuteJavascriptWithResult("jwplayer().getPlaylistItem().index"));
                            int maxIndex = int.Parse(webControl.ExecuteJavascriptWithResult("jwplayer().getPlaylist().length-1"));
                            index += 1;
                            if (index > maxIndex)
                                index = 0;
                            webControl.ExecuteJavascript("jwplayer().playlistItem(" + index + ")");
                        }));
                }
                Thread.Sleep(20); //i think its ok to check 50 times a min
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            keyboard = new Thread(keyboardThread);
            keyboard.Start();
            ResendSongData();

            timer1.Enabled = true;
            
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            keyboard.Abort();
            Communicator.GetInstance().Clear();
            base.OnClosing(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!finishedLoading)
                return;
            try
            {
                CurrentSong = webControl.ExecuteJavascriptWithResult("document.getElementById(\"txtSongTitleRight\").outerText");
                CurrentArtist = webControl.ExecuteJavascriptWithResult("document.getElementById(\"txtSongAuthorRight\").outerText");
            }
            catch (NullReferenceException) { }
            
        }

        private void ResendSongData() //just change it to make the property send the data again
        {
            Communicator.GetInstance().Clear();
            Communicator.GetInstance().SendTextUnlimitedTime("F1:Mute", 14, 300, 20,new Argb(200,200,200,200));
            Communicator.GetInstance().SendTextUnlimitedTime("F2:VDown", 14, 400, 20, new Argb(200, 200, 200, 200));
            Communicator.GetInstance().SendTextUnlimitedTime("F3:VUp", 14, 500, 20, new Argb(200, 200, 200, 200));
            Communicator.GetInstance().SendTextUnlimitedTime("F5:Prev", 14, 700, 20, new Argb(200, 200, 200, 200));
            Communicator.GetInstance().SendTextUnlimitedTime("F6:Pause", 14, 800, 20, new Argb(200, 200, 200, 200));
            Communicator.GetInstance().SendTextUnlimitedTime("F7:Next", 14, 900, 20, new Argb(200, 200, 200, 200));
            string oldSong = CurrentSong, oldArtist = CurrentArtist;
            resendArtist = resendSong = true;
            CurrentSong = oldSong;
            CurrentArtist = oldArtist;
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            try
            {
                Injector.GetInstnace().Inject("League of Legends", Directory.GetCurrentDirectory() + "\\"+dllName);
                System.Threading.Thread.Sleep(2000);
                Communicator.GetInstance().SendText("Injected Successfuly", 3000, 20, 50, 50);
                ResendSongData();

            }
            catch (Exception ex)
            {
                if (ex is InvalidProcess)
                {
                    MessageBox.Show("LoL not running");
                }
                else if(ex is InvalidDllPath)
                {
                    MessageBox.Show("Wrong Dll Path");
                }
                else if (ex is AlreadyInjected)
                {
                    MessageBox.Show("Already injected to LoL!");
                }
            }
        }

        private void Awesomium_Windows_Forms_WebControl_LoadingFrameComplete(object sender, FrameEventArgs e)
        {
            finishedLoading = true;
        }

        private void Awesomium_Windows_Forms_WebControl_LoadingFrame(object sender, LoadingFrameEventArgs e)
        {
            finishedLoading = false;
        }
        


    }
}
