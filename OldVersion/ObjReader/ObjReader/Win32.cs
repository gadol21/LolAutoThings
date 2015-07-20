using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ObjectReader
{
    class Win32
    {
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int smIndex);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
             ProcessAccessFlags processAccess,
             bool bInheritHandle,
             int processId
        );

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
        [Flags]
        public enum SnapshotFlags : int
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            All = (HeapList | Process | Thread | Module),
            //Inherit = 0x80000000,
            //NoHeaps = 0x40000000

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEENTRY32
        {
            private const int MAX_PATH = 255;
            internal uint dwSize;    //размер структуры в байтах
            internal uint th32ModuleID;//ID модуля
            internal uint th32ProcessID;//ID процесса модуля
            internal uint GlblcntUsage;//общее для системы количество обращений к модулю
            internal uint ProccntUsage;//количество обращений к модулю, сделанных из текущего процесса
            internal IntPtr modBaseAddr;//адрес модуля в адресном пространстве процесса
            internal uint modBaseSize;//размер модуля
            internal IntPtr hModule;//дескриптор модуля в контексте процесса
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
            internal string szModule;//имя модуля
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 5)]
            internal string szExePath;//полный путь к файлу
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            int lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
