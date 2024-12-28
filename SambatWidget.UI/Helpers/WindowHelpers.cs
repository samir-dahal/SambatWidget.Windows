using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

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
                    break;
                }
            }
        }
        public static void CalculateWindowMoveOffset(this Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            var hwnd = windowInteropHelper.Handle;

            // Determine the screen where the window is currently located
            var screen = Screen.FromHandle(hwnd);

            // Get the bounds and working area of the current screen
            var screenBounds = screen.Bounds;
            var workingArea = screen.WorkingArea;

            // Convert bounds and working area to WPF coordinates
            var dpiScaleX = VisualTreeHelper.GetDpi(window).DpiScaleX;
            var dpiScaleY = VisualTreeHelper.GetDpi(window).DpiScaleY;

            double workingAreaLeft = workingArea.Left / dpiScaleX;
            double workingAreaTop = workingArea.Top / dpiScaleY;
            double workingAreaWidth = workingArea.Width / dpiScaleX;
            double workingAreaHeight = workingArea.Height / dpiScaleY;

            double screenBoundsLeft = screenBounds.Left / dpiScaleX;
            double screenBoundsTop = screenBounds.Top / dpiScaleY;
            double screenBoundsWidth = screenBounds.Width / dpiScaleX;
            double screenBoundsHeight = screenBounds.Height / dpiScaleY;

            // Determine the bounds based on AllowGlobalPosition
            double allowedTop = App.Setting.AllowGlobalPosition ? screenBoundsTop : workingAreaTop;
            double allowedLeft = App.Setting.AllowGlobalPosition ? screenBoundsLeft : workingAreaLeft;
            double allowedBottom = App.Setting.AllowGlobalPosition
                ? screenBoundsTop + screenBoundsHeight
                : workingAreaTop + workingAreaHeight;
            double allowedRight = App.Setting.AllowGlobalPosition
                ? screenBoundsLeft + screenBoundsWidth
                : workingAreaLeft + workingAreaWidth;

            // Adjust window position based on the calculated bounds
            window.Top = Math.Max(allowedTop, Math.Min(allowedBottom - window.ActualHeight, window.Top));
            window.Left = Math.Max(allowedLeft, Math.Min(allowedRight - window.ActualWidth, window.Left));
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
        private static List<Window> _openWindows = new List<Window>();
        private static bool IsWindowOpen(Window window)
        {
            var openedWindow = _openWindows.FirstOrDefault(w => w.GetType() == window.GetType());
            if (openedWindow != null)
            {
                openedWindow.Activate();
                return true;
            }
            return false;
        }
        public static List<Window> GetOpenWindows()
        {
            return _openWindows;
        }
        public static void OpenWindow(this Window window, bool dialog = false)
        {
            if (IsWindowOpen(window)) return;
            window.Closed += (s, e) => _openWindows.Remove(window);
            _openWindows.Add(window);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (!dialog)
            {
                window.Show();
            }
            else
            {
                window.ShowDialog();
            }
        }
    }
}