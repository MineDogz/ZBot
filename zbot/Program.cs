using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using System.Drawing;
using System.Collections;

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
            Thread.Sleep(1000);
            process = GetProcess();
            Thread.Sleep(1000);
            if (process != null)
            {
                Log("Found!");
                Thread.Sleep(1000);
                SetWindow(process, windowX, windowY, windowWidth, windowHeight);
                Thread.Sleep(1000);
                DoMouse(windowWidth / 2, windowHeight / 2);
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
        static int GetProcessId()
        {
            return process.Id;
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
        [DllImport("User32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        public static void SetWindow(Process p, int x, int y, int width, int height)
        {
            IntPtr window = FindWindowByCaption(IntPtr.Zero, p.MainWindowTitle);
            SetWindowPos(window, IntPtr.Zero, x, y, width, height, 0);
            SwitchToThisWindow(window, true);
        }

        //
        // Fake mouse
        //
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point) => new Point(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        // For your convenience
        public static Point GetCursorPosition()
        {
            PointInter lpPoint;
            GetCursorPos(out lpPoint);
            return (Point)lpPoint;
        }

        public static void MoveCursorToPoint(int x, int y)
        {
            SetCursorPos(x, y);
        }

        private static void DoMouse(int x, int y)
        {
            
            Point target = new Point(x, y);
            Point point = GetCursorPosition();
            

            int range = 10;
            bool inside = false;

            if (target.X >= point.X - range && target.X <= point.X + range)
            {
                if (target.Y >= point.Y - range && target.Y <= point.Y + range)
                {
                    Log("In Range");
                    inside = true;
                }
            }


            if (inside)
            {
                LeftClick();
            }
            else
            {
                Log("Moving to location");
                Point next = Lerpz(point, target, 0.05);
                MoveCursorToPoint(next.X, next.Y);
                Thread.Sleep(10);
                Draw(Brushes.Red, point.X, point.Y, 10, 10);
                Draw(Brushes.Blue, target.X, target.Y, 10, 10);
                DoMouse(x, y);
            }
           

        }

        private static int Lerp(int firstFloat, int secondFloat, double by)
        {
            return Convert.ToInt32(firstFloat * (1 - by) + secondFloat * by);
        }
        private static Point Lerpz(Point firstVector, Point secondVector, double by)
        {
            int retX = Lerp(firstVector.X, secondVector.X, by);
            int retY = Lerp(firstVector.Y, secondVector.Y, by);
            return new Point(retX, retY);
        }


        //
        // Mouse click events
        //
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        public static void LeftClick()
        {
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }


        //
        // Draw
        //
        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

        static void Draw(Brush brush, int x, int y, int w, int h)
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                ArrayList al = new ArrayList();
                g.FillEllipse(brush, Convert.ToInt32(x * 1.5), Convert.ToInt32(y * 1.5), w, h);
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }
    }
}
