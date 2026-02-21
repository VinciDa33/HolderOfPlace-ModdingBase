using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModdingCore
{
    internal class Console
    {
        private const uint StdOutputHandle = 4294967285u;

        private const int MY_CODE_PAGE = 437;

        private const uint GENERIC_WRITE = 1073741824u;

        private const uint FILE_SHARE_WRITE = 2u;

        private const uint OPEN_EXISTING = 3u;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(uint nStdHandle, IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        public static void LoadConsole()
        {
            AllocConsole();
            IntPtr handle = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, 0u, OPEN_EXISTING, 0u, 0u);
            SetStdHandle(StdOutputHandle, handle);
            TextWriter @out = new StreamWriter(System.Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            System.Console.SetOut(@out);
            System.Console.WriteLine("Hello world!");
            Application.logMessageReceived += OnApplicationOnlogMessageReceived;
            System.Console.WriteLine(Application.streamingAssetsPath);
        }

        private static void OnApplicationOnlogMessageReceived(string logString, string stackTrace, LogType type)
        {
            System.Console.WriteLine($"[{type}] {logString}");
            if (type == LogType.Error || type == LogType.Exception)
            {
                System.Console.WriteLine(stackTrace);
                ErrorPopup.Open(logString, stackTrace);
            }
        }

        public static void UnloadConsole()
        {
            System.Console.WriteLine("This console is not valid. If you see this, you can safely close the window.");
            FreeConsole();
            Application.logMessageReceived -= OnApplicationOnlogMessageReceived;
        }
    }
}
