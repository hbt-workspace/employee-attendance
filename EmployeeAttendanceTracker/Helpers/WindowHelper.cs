using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace EmployeeAttendanceTracker.Helpers;

// Helper class for window management.
public sealed class WindowHelper
{
    #region Constants

    private const int WM_GETMINMAXINFO = 0x0024;
    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    #endregion

    #region Window Initialization

    public void Attach(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.SourceInitialized += (_, _) =>
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            HwndSource? source = HwndSource.FromHwnd(handle);

            source?.AddHook(WindowProc);
        };
    }

    #endregion

    #region Window State Controls

    public void Minimize(Window window)
    {
        window.WindowState = WindowState.Minimized;
    }

    public void ToggleMaximize(Window window, Button maximizeButton)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            Restore(window, maximizeButton);
        }
        else
        {
            Maximize(window, maximizeButton);
        }
    }

    public void Close(Window window)
    {
        window.Close();
    }

    private static void Maximize(Window window, Button maximizeButton)
    {
        window.WindowState = WindowState.Maximized;
        maximizeButton.Content = "❐";
        maximizeButton.ToolTip = "Restore Down";
    }

    private static void Restore(Window window, Button maximizeButton)
    {
        window.WindowState = WindowState.Normal;
        maximizeButton.Content = "□";
        maximizeButton.ToolTip = "Maximize";
    }

    public void HandleHeaderMouseLeftButtonDown(
        Window window,
        Button maximizeButton,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;

        if (e.ClickCount == 2)
        {
            ToggleMaximize(window, maximizeButton);
            return;
        }

        if (e.ButtonState == MouseButtonState.Pressed)
        {
            try
            {
                window.DragMove();
            }
            catch (InvalidOperationException)
            {
                // Ignore if dragging ends before it starts.
            }
        }
    }

    #endregion

    #region Taskbar-Aware Maximization

    private static IntPtr WindowProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            UpdateMaximizedBounds(hwnd, lParam);
            handled = true;
        }

        return IntPtr.Zero;
    }

    private static void UpdateMaximizedBounds(
        IntPtr hwnd,
        IntPtr lParam)
    {
        IntPtr monitor = MonitorFromWindow(
            hwnd,
            MONITOR_DEFAULTTONEAREST);

        if (monitor == IntPtr.Zero)
            return;

        MONITORINFO monitorInfo = new()
        {
            cbSize = Marshal.SizeOf<MONITORINFO>()
        };

        if (!GetMonitorInfo(monitor, ref monitorInfo))
            return;

        MINMAXINFO info =
            Marshal.PtrToStructure<MINMAXINFO>(lParam);

        info.ptMaxPosition.x =
            monitorInfo.rcWork.Left - monitorInfo.rcMonitor.Left;

        info.ptMaxPosition.y =
            monitorInfo.rcWork.Top - monitorInfo.rcMonitor.Top;

        info.ptMaxSize.x =
            monitorInfo.rcWork.Right - monitorInfo.rcWork.Left;

        info.ptMaxSize.y =
            monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top;

        Marshal.StructureToPtr(info, lParam, false);
    }

    #endregion

    #region Windows API

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(
        IntPtr hwnd,
        uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(
        IntPtr hMonitor,
        ref MONITORINFO lpmi);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    #endregion
}
