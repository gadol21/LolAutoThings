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
            buffer[0] = (byte)'c';
            buffer[1] = (byte)'l';
            buffer[2] = (byte)'e';
            buffer[3] = (byte)'a';
            buffer[4] = (byte)'r';
            sock.SendTo(buffer, ep);
        }
        public void SendText(string text, int millis, int fontSize, int x, int y, Argb color,TextFormat format = TextFormat.Left)
        {
            Byte[] buffer = new Byte[28 + text.Length + 1];

            Byte[] textAscii = new Byte[4];
            buffer[0] = (byte)'t';
            buffer[1] = (byte)'e';
            buffer[2] = (byte)'x';
            buffer[3] = (byte)'t';

            Byte[] arrFsize = BitConverter.GetBytes(fontSize);
            Byte[] arrMillis = BitConverter.GetBytes(millis);
            Byte[] arrX = BitConverter.GetBytes(x);
            Byte[] arrY = BitConverter.GetBytes(y);
            Byte[] arrFormat = BitConverter.GetBytes((int)format);
            for (int i = 0; i < 4; i++)
            {
                buffer[i + 4] = arrMillis[i];
                buffer[i + 8] = arrFsize[i];
                buffer[i + 12] = arrX[i];
                buffer[i + 16] = arrY[i];
                buffer[i + 24] = arrFormat[i];
            }
            buffer[20] = color.alpha;
            buffer[21] = color.red;
            buffer[22] = color.green;
            buffer[23] = color.blue;

            for (int i = 0; i < text.Length; i++)
                buffer[28 + i] = (byte)text[i];
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
            for (int i = 0; i < text.Length; i++)
                buffer[i] = (byte)text[i];
            buffer[buffer.Length - 1] = 0; //null terminate the string
            sock.SendTo(buffer, ep);
        }
#if LEAGUEOFLEGENDS
        internal void LoLFloatingText(uint unitBase, string message, uint messageType)
        {
            Byte[] buffer = new Byte[16 + message.Length + 1];
            string command = "floating";
            for (int i = 0; i < command.Length;i++ )
                buffer[i] = (byte)command[i];
            Byte[] arrUnitBase = BitConverter.GetBytes(unitBase);
            Byte[] arrMessageType = BitConverter.GetBytes(messageType);
            for (int i = 0; i < 4; i++)
            {
                buffer[8 + i] = arrUnitBase[i];
                buffer[12 + i] = arrMessageType[i];
            }
            for (int i = 0; i < message.Length; i++)
                buffer[16 + i] = (byte)message[i];
            buffer[buffer.Length - 1] = 0; //null terminate the string
            sock.SendTo(buffer, ep);
        }
#endif

    }
}
