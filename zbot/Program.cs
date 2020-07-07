using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace zbot
{
    class Program
    {
        //
        // privates
        //
        static private string title = "ZBot";
        static private string processName = "rs2client";
        static private Process process = null;
        static private int windowWidth = 1920;
        static private int windowHeight = 1080;
        static private int windowX = 0;
        static private int windowY = 0;

        //
        // Main thread
        //
        static void Main(string[] args)
        {
            Start();
            process = GetProcess();
            if (process != null)
            {
                Log("Found!");
                SetWindow(process, windowX, windowY, windowWidth, windowHeight);
            }
            Console.ReadLine();
        }

        //
        // Easyer to log.
        //
        static void Log(string input)
        {
            Console.WriteLine(input);
        }

        //
        // First function
        //
        static void Start()
        {
            Log("Started!");
            Log("Setting title: " + title);
            Console.Title = title;
            Log("Title set.");
        }

        //
        // Find window.
        //
        static Process GetProcess()
        {
            Log("Getting process: " + processName);
            try
            {
                return Process.GetProcessesByName(processName)[0];
            }
            catch (Exception)
            {
                //Log(err.Message);
                Log("Could not find runescape :(");
                return null;
            }
        }

        //
        // get desktop window
        //
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        //
        // set desktop window
        //
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            SynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        //
        // Set window function
        //
        public static void SetWindow(Process p, int x, int y, int width, int height)
        {
            IntPtr window = FindWindowByCaption(IntPtr.Zero, p.MainWindowTitle);
            SetWindowPos(window, IntPtr.Zero, x, y, width, height, 0);
        }
    }
}
