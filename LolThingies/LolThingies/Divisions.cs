using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using lolcomjector;
using ObjectReader;
using Awesomium.Core;
using Awesomium.Windows.Forms;

namespace LolThingies
{
    [Obsolete("Not working at the moment")]
    class Divisions:Module
    {
        string world = "euw";
        string key = "af5dd05c-4f51-44db-bb5f-24ba90b5bbf5";
        int counter = 0;
        WebControl webControl;
        Dictionary<int, string> dictionary;
        Dictionary<string, string> divisionPerPlayer;

        private Thread thread;
        private string text;

        public Divisions(Keys key, int x, int y)
            : base(key, x, y)
        {
            text = "Loading";
            webControl = new WebControl();
            webControl.LoadingFrameComplete += new Awesomium.Core.FrameEventHandler(webControl_LoadingFrameComplete);
        }

        public override void Init()
        {
            Console.WriteLine("Started "+name);
            thread = new Thread(GetDivisions);
            thread.Start();
        }
        public override void KeyPress()
        {
            if (on)
            {
                Communicator.GetInstance().SendTextUnlimitedTime(text, 20, Win32.GetSystemMetrics(0)/2, 5,TextFormat.Center);
            }
            else
            {
                Communicator.GetInstance().RemoveText(text);
            }
        }
        public override void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                for (int i = 0; i < 5; i++)
                    Communicator.GetInstance().RemoveText("Ignite WILL KILL");
            }
        }
        private void GetDivisions()
        {
            while (Win32.FindWindow(null, "League of Legends (TM) Client") == IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(500);
            }
            while (LoLReader.IsOnLoadingScreen())
            {
                Thread.Sleep(500);
            }
            List<Unit> players = LoLReader.GetAllChampions();
            List<string> names = new List<string>();
            foreach (Unit u in players)
            {
                names.Add(u.name);
            }
            string[] namesArr = names.ToArray();
            string s = "";
            for (int i = 0; i < namesArr.Length - 1; i++)
                s += namesArr[i] + ",";
            s += namesArr[namesArr.Length - 1];
            //string s = "cg potato,tapudi,suprizez";
            webControl.Source = new Uri("https://" + world + ".api.pvp.net/api/lol/" + world + "/v1.4/summoner/by-name/" + s + "?api_key="+key);
            Console.WriteLine("https://" + world + ".api.pvp.net/api/lol/" + world + "/v1.4/summoner/by-name/" + s + "?api_key="+key);
        }

        void webControl_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            Console.WriteLine("Loading complete");
            if (counter == 0)
            {
                string s = webControl.ExecuteJavascriptWithResult("document.body.innerText");
                string[] players = s.Split(new string[] { "}," }, StringSplitOptions.None);
                dictionary = new Dictionary<int, string>();
                string ids = "";
                foreach (string player in players)
                {
                    int idPos = player.IndexOf("\"id\":") + 5;
                    string idString = player.Substring(idPos, player.IndexOf(',', idPos) - idPos);
                    int id = int.Parse(idString);
                    int namePos = player.IndexOf("\"name\":") + 7;
                    string name = player.Substring(namePos, player.IndexOf(',', namePos) - namePos).Trim(new char[] { '\"' });
                    dictionary.Add(id, name);
                    ids += "," + id.ToString();
                }
                ids = ids.Substring(1);
                string str = "https://" + world + ".api.pvp.net/api/lol/" + world + "/v2.5/league/by-summoner/" + ids + "/entry?api_key=af5dd05c-4f51-44db-bb5f-24ba90b5bbf5";
                webControl.Source = new Uri(str);
                Console.WriteLine(str);
            }
            else if (counter == 1)
            {
                string page = webControl.ExecuteJavascriptWithResult("document.body.innerText");
                string para;
                string nextPara;
                int index;
                foreach (KeyValuePair<int, string> pair in dictionary)
                {
                    string division, league;
                    string isUnranked;
                    try
                    {
                        int start = page.IndexOf(pair.Key.ToString());
                        isUnranked = page.Substring(start, page.IndexOf("RANKED_TEAM", start) - start);
                        if (isUnranked.IndexOf("RANKED_SOLO_5x5") == -1)
                            throw new Exception();
                        para = "\"division\":";
                        nextPara = ",";
                        index = page.IndexOf(para, start);
                        division = page.Substring(index + para.Length, page.IndexOf(nextPara, index) - (index + para.Length)).Trim(new char[] { '\"' });

                        para = "\"tier\":";
                        nextPara = ",";
                        index = page.IndexOf(para, start);
                        league = page.Substring(index + para.Length, page.IndexOf(nextPara, index) - (index + para.Length)).Trim(new char[] { '\"' });
                    }
                    catch (Exception)
                    {
                        //Check if its team or not
                        league = "Unranked";
                        division = "";
                    }
                    Console.WriteLine(pair.Value + ":" + league + " " + division);
                    divisionPerPlayer.Add(pair.Value, league + " " + division);
                }
                string team1 = "";
                string team2 = "";
                List<Unit> players = LoLReader.GetAllChampions();
                foreach (Unit player in players)
                {
                    if (player.team == Team.Team1)
                        team1 += player.name + "\t" + divisionPerPlayer[player.name];
                    else
                        team2 += player.name + "\t" + divisionPerPlayer[player.name];
                }
                Communicator.GetInstance().RemoveText(text);
                text = team1 + "\t|\t" + team2;
                Communicator.GetInstance().SendTextUnlimitedTime(text, 20, Win32.GetSystemMetrics(0) / 2, 5, TextFormat.Center);
            }
            counter += 1;
        }
    }
}
