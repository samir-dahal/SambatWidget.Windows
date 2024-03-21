using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace SambatWidget.UI.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    public static class WindowHelpers
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder className, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public static void SetWindowsPos(this Window window, bool topMost = true)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (topMost)
            {
                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }
        public static void HideWindowFromAltTab(this Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            IntPtr oldStyle;
            if (IntPtr.Size == 4)
            {
                oldStyle = GetWindowLongPtr32(handle, GWL_EXSTYLE);
                SetWindowLongPtr32(handle, GWL_EXSTYLE, oldStyle.ToInt32() | WS_EX_TOOLWINDOW);
            }
            else
            {
                oldStyle = GetWindowLongPtr64(handle, GWL_EXSTYLE);
                SetWindowLongPtr64(handle, GWL_EXSTYLE, new IntPtr(oldStyle.ToInt64() | WS_EX_TOOLWINDOW));
            }
        }
        public static void MoveWindowToTopMost(this Window window)
        {
            const int HWND_TOPMOST = -1;
            //const int HWND_NOTOPMOST = -2;
            const string SHELLTRAY = "Shell_traywnd";

            WindowInteropHelper thisWin = new WindowInteropHelper(window);
            IntPtr shellTray = NativeMethods.GetWindowByClassName(IntPtr.Zero, SHELLTRAY);

            //Reassign owner when explorer.exe restarts
            if (thisWin.Owner != shellTray)
            {
                thisWin.Owner = shellTray;
            }

            //check if window is behind the taskbar. If yes, bring it in front without activating it.
            for (IntPtr h = thisWin.Handle; h != IntPtr.Zero; h = NativeMethods.GetWindow(h, (uint)NativeMethods.GW.HWNDPREV))
            {
                if (h == shellTray)
                {
                    Debug.WriteLine("this window is behind Shell_TrayWnd");
                    NativeMethods.SetWindowPos(thisWin.Handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0,
                        (int)NativeMethods.SWP.ASYNCWINDOWPOS |
                        (int)NativeMethods.SWP.NOACTIVATE |
                        (int)NativeMethods.SWP.NOMOVE |
                        (int)NativeMethods.SWP.NOSIZE);
                    //NativeMethods.SetWindowPos(thisWin.Handle, (IntPtr)HWND_NOTOPMOST, 0, 0, 0, 0, SWP_ASYNCWINDOWPOS | SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
                    break;
                }
            }
        }
        public static void CalculateWindowMoveOffset(this Window window)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;

            double top = App.Setting.AllowGlobalPosition ? SystemParameters.VirtualScreenHeight : desktopWorkingArea.Bottom;
            double left = App.Setting.AllowGlobalPosition ? SystemParameters.VirtualScreenWidth : desktopWorkingArea.Right;
            
            window.Top = Math.Max(0, Math.Min(top - window.ActualHeight, window.Top));
            window.Left = Math.Max(0, Math.Min(left - window.ActualWidth, window.Left));
        }
        public static bool IsForegroundFullscreen()
        {
            // Pre-allocate 256 characters, since this is the maximum class name length.
            StringBuilder ClassName = new StringBuilder(256);

            bool runningFullScreen = false;

            IntPtr hWnd = GetForegroundWindow();

            GetClassName(hWnd, ClassName, ClassName.Capacity);
            //Check we haven't picked up the desktop or the shell
            if (hWnd != GetDesktopWindow() && ClassName.ToString() != "WorkerW" && hWnd != GetShellWindow())
            {
                GetWindowRect(hWnd, out RECT rcApp);    //Get the coordinates of the foreground window
                GetWindowRect(GetDesktopWindow(), out RECT rcDesk); //Get the coordinates of the entire screen based on the desktop window handle
                if (rcApp.Left <= rcDesk.Left &&
                    rcApp.Top <= rcDesk.Top &&
                    rcApp.Right >= rcDesk.Right &&
                    rcApp.Bottom >= rcDesk.Bottom)
                {
                    runningFullScreen = true;
                }
            }
            return runningFullScreen;
        }
    }
}