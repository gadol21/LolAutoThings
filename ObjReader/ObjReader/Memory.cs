using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    class Memory
    {
        public static int ReadInt(IntPtr process, int adress, byte[] buffer)
        {
            int bytesRead = 0;
            Win32.ReadProcessMemory((int)process, adress, buffer, 4, ref bytesRead);
            int value = BitConverter.ToInt32(buffer, 0);
            return value;
        }
        public static byte ReadByte(IntPtr process, int adress, byte[] buffer)
        {
            int bytesRead = 0;
            Win32.ReadProcessMemory((int)process, adress, buffer, 1, ref bytesRead);
            return buffer[0];
        }
        public static float ReadFloat(IntPtr process, int adress, byte[] buffer)
        {
            int bytesRead = 0;
            Win32.ReadProcessMemory((int)process, adress, buffer, 4, ref bytesRead);
            float value = BitConverter.ToSingle(buffer, 0);
            return value;
        }
        public static string ReadString(IntPtr process, int adress, byte[] buffer, int size)
        {
            int bytesRead = 0;
            byte[] obj = new byte[size];
            Win32.ReadProcessMemory((int)process, adress, obj, size, ref bytesRead);
            return System.Text.Encoding.UTF8.GetString(obj);
        }
        public static string ReadString(IntPtr process, int adress, byte[] buffer)
        {
            int bytesRead = 0;
            char lastChar = (char)255;
            int counter = 0;
            string str = "";
            while (lastChar != 0)
            {
                byte[] stringByte = new byte[1];
                Win32.ReadProcessMemory((int)process, adress + counter, stringByte, 1, ref bytesRead);
                lastChar = (char)stringByte[0];
                counter += 1;
                if (lastChar != 0)
                    str += (char)stringByte[0];
            }
            return str;
        }
        public static void WriteFloat(IntPtr process, int adress, float value)
        {
            int bytesRead = 0;
            byte[] buffer = BitConverter.GetBytes(value);
            Win32.WriteProcessMemory(process, adress, buffer, 4, out bytesRead);
        }
    }
}
