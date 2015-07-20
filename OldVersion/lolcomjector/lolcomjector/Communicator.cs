#define LEAGUEOFLEGENDS //remove this to remove League specific functions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("ObjReader")] //so that objreader could see LoLFloatingText, but other assemblys couldn't
namespace lolcomjector
{
    public struct Argb
    {
        public byte alpha, red, green, blue;
        public Argb(byte alpha, byte red, byte green, byte blue)
        {
            this.alpha = alpha;
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
    public enum TextFormat : int
    {
        Center = 1,
        Left = 0,
        Right = 2
    }

    public struct Position{
        public float x;
        public float z;
        public float y;

        public Position(float x, float z, float y){
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public enum MoveType : int
    {
        Move = 2,
        Attack = 3,
        AttackMove = 0,
        Stop = 0xA
    }

    public class Communicator
    {
        private const int WAIT_INFINITE = -10000;
        private static Communicator instance = null;
        private Socket sock;
        private IPEndPoint ep;
        private Communicator()
        {

            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Blocking = false; //i don't wanna make a thread for it :(
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2501);
        }
        public static Communicator GetInstance()
        {
            if (instance == null)
                instance = new Communicator();
            return instance;
        }
        public void Clear()
        {
            Byte[] buffer = new Byte[5];
            Encoding.ASCII.GetBytes("clear").CopyTo(buffer, 0);
            sock.SendTo(buffer, ep);
        }
        public void SendText(string text, int millis, int fontSize, int x, int y, Argb color,TextFormat format = TextFormat.Left)
        {
            Byte[] buffer = new Byte[28 + text.Length + 1];

            Encoding.ASCII.GetBytes("text").CopyTo(buffer, 0);

            BitConverter.GetBytes(millis).CopyTo(buffer, 4);
            BitConverter.GetBytes(fontSize).CopyTo(buffer, 8);
            BitConverter.GetBytes(x).CopyTo(buffer, 12);
            BitConverter.GetBytes(y).CopyTo(buffer, 16);
            BitConverter.GetBytes((int)format).CopyTo(buffer, 24);

            buffer[20] = color.alpha;
            buffer[21] = color.red;
            buffer[22] = color.green;
            buffer[23] = color.blue;

            Encoding.ASCII.GetBytes(text).CopyTo(buffer, 28);
            buffer[buffer.Length - 1] = 0;//null terminate the string
            sock.SendTo(buffer, ep);
        }
        public void SendText(string text, int millis, int fontSize, int x, int y, TextFormat format = TextFormat.Left)
        {
            SendText(text, millis, fontSize, x, y, new Argb(255, 255, 0, 0),format); //default color is red
        }
        public void SendTextUnlimitedTime(string text, int fontSize, int x, int y,TextFormat format = TextFormat.Left)
        {
            SendTextUnlimitedTime(text, fontSize, x, y, new Argb(255, 255, 0, 0),format); //default color is red
        }
        public void SendTextUnlimitedTime(string text, int fontSize, int x, int y, Argb color, TextFormat format = TextFormat.Left)
        {
            SendText(text, WAIT_INFINITE, fontSize, x, y, color,format);
        }
        public void RemoveText(string text)
        {
            text = "remove" + text;
            Byte[] buffer = new Byte[text.Length + 1];
            Encoding.ASCII.GetBytes(text).CopyTo(buffer, 0);
            buffer[buffer.Length - 1] = 0; //null terminate the string
            sock.SendTo(buffer, ep);
        }
#if LEAGUEOFLEGENDS
        internal void LolFloatingText(uint unitBase, string message, uint messageType)
        {
            Byte[] buffer = new Byte[16 + message.Length + 1];
            Encoding.ASCII.GetBytes("floating").CopyTo(buffer, 0);
            BitConverter.GetBytes(unitBase).CopyTo(buffer,8);
            BitConverter.GetBytes(messageType).CopyTo(buffer,12);
            Encoding.ASCII.GetBytes(message).CopyTo(buffer, 16);
            buffer[buffer.Length - 1] = 0; //null terminate the string
            sock.SendTo(buffer, ep);
        }

        internal void LolMoveTo(float x, float z, float y, MoveType moveType, uint myChamp, uint targetUnit)
        {
            byte[] buffer = new byte[30];
            Encoding.ASCII.GetBytes("moveto").CopyTo(buffer, 0); //make it look like this everywhere in this file

            BitConverter.GetBytes(x).CopyTo(buffer, 6);
            BitConverter.GetBytes(y).CopyTo(buffer, 10);
            BitConverter.GetBytes(z).CopyTo(buffer, 14);

            BitConverter.GetBytes((int)moveType).CopyTo(buffer, 18);
            BitConverter.GetBytes(myChamp).CopyTo(buffer, 22);
            BitConverter.GetBytes(targetUnit).CopyTo(buffer, 26);

            sock.SendTo(buffer, ep);
        }
#endif

    }
}
