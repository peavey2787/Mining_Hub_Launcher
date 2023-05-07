using System;
using System.Runtime.InteropServices;
namespace Mining_Hub_Launcher
{
    public class WindowHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x80;
        const int SW_RESTORE = 9;
        const int SW_MINIMIZE = 6;

        public static void BringToFront(string windowTitle)
        {
            var hwnd = FindWindow(null, windowTitle);
            SetForegroundWindow(hwnd);
            ShowWindow(hwnd, SW_RESTORE);
        }
        public static void BringToFront(IntPtr hWnd)
        {
            SetForegroundWindow(hWnd);
            ShowWindow(hWnd, SW_RESTORE);
        }
        public static void HideWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_MINIMIZE);

            // Remove the window from the taskbar
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
        }
    }
}
