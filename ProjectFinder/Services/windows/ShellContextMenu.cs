#if WINDOWS

using System.Runtime.InteropServices;
using System.Text;


namespace ProjectFinder.Services.Windows
{
    public class ShellContextMenuHelper
    {
        private const uint CMF_NORMAL = 0x00000000;
        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_RIGHTBUTTON = 0x0002;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        protected static extern int SHParseDisplayName(string name, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

        [DllImport("shell32.dll")]
        protected static extern int SHBindToParent(IntPtr pidl, ref Guid riid, out IntPtr ppv, out IntPtr ppidlLast);

        [DllImport("user32.dll")]
        protected static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll")]
        protected static extern bool DestroyMenu(IntPtr hMenu);
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern uint TrackPopupMenuEx(
            IntPtr hMenu,
            uint uFlags,
            int x,
            int y,
            IntPtr hWnd,
            IntPtr lpTPMParams);

        // COM Interfaces
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214e4-0000-0000-c000-000000000046")]
        protected interface IContextMenu
        {
            [PreserveSig]
            int QueryContextMenu(IntPtr hmenu, uint indexMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);

            void InvokeCommand(ref CMINVOKECOMMANDINFOEX pici);

            void GetCommandString(IntPtr idCmd, uint uType, IntPtr pReserved, StringBuilder pszName, uint cchMax);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F4-0000-0000-C000-000000000046")]
        protected interface IContextMenu2 : IContextMenu
        {
            [PreserveSig]
            int HandleMenuMsg(uint uMsg, IntPtr wParam, IntPtr lParam);
        }
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("BCFCE0A0-EC17-11d0-8D10-00A0C90F2719")]
        protected interface IContextMenu3 : IContextMenu2
        {
            [PreserveSig]
            int QueryContextMenu(IntPtr hMenu, uint indexMenu, int idCmdFirst, int idCmdLast, uint uFlags);
            [PreserveSig]
            new void InvokeCommand(ref CMINVOKECOMMANDINFOEX pici);
            [PreserveSig]
            void GetCommandString(uint idCmd, uint uType, uint pReserved, [MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName, uint cchMax);
            [PreserveSig]
            new int HandleMenuMsg(uint uMsg, IntPtr wParam, IntPtr lParam);
            [PreserveSig]
            int HandleMenuMsg2(uint uMsg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);
        }




        // Struct definition
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        protected struct CMINVOKECOMMANDINFOEX
        {
            public int cbSize;
            public int fMask;
            public IntPtr hwnd;
            public IntPtr lpVerb;        // <- MUST be IntPtr (not string)
            public IntPtr lpParameters;
            public IntPtr lpDirectory;
            public int nShow;
            public int dwHotKey;
            public IntPtr hIcon;
            public IntPtr lpTitle;
            public IntPtr lpVerbW;
            public IntPtr lpParametersW;
            public IntPtr lpDirectoryW;
            public IntPtr lpTitleW;
            public POINT ptInvoke;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct POINT
        {
            public int x;
            public int y;
        }
        [DllImport("user32.dll")]
        protected static extern IntPtr GetForegroundWindow();

        protected static IntPtr MAKEINTRESOURCE(int i)
        {
            return (IntPtr)(i & 0xFFFF);
        }

        // P/Invoke
        protected static IntPtr SubclassProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
        {
            if (dwRefData != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(dwRefData);
                if (handle.Target is IContextMenu3 icm3)
                {
                    icm3.HandleMenuMsg2(msg, wParam, lParam, out _);
                }
            }

            return DefSubclassProc(hWnd, msg, wParam, lParam);
        }
        [DllImport("comctl32.dll")]
        protected static extern IntPtr DefSubclassProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("comctl32.dll", SetLastError = true)]
        protected static extern bool SetWindowSubclass(
     IntPtr hWnd,
     SUBCLASSPROC pfnSubclass,
     IntPtr uIdSubclass,
     IntPtr dwRefData
 );

        protected delegate IntPtr SUBCLASSPROC(
            IntPtr hWnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr uIdSubclass,
            IntPtr dwRefData
        );
        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        protected static void CloseContextMenu()
        {
            // Get the focused window, which should be the menu's owner
            IntPtr hWnd = GetFocus();

            if (hWnd != IntPtr.Zero)
            {

                PostMessage(hWnd, 0x001F, IntPtr.Zero, IntPtr.Zero);
                Console.WriteLine("WM_CANCELMODE sent to close context menu.");
            }
            else
            {
                Console.WriteLine("No focused window to send cancel.");
            }
        }
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // Main entry
        protected static void Show(string filePath, int screenX, int screenY)
        {
            if (!OperatingSystem.IsWindows() || (!File.Exists(filePath) && !Directory.Exists(filePath)))
                return;

            IntPtr pidl = IntPtr.Zero;
            IntPtr pidlParent = IntPtr.Zero;
            IntPtr pidlChild = IntPtr.Zero;
            IntPtr contextMenuPtr = IntPtr.Zero;
            IntPtr hMenu = IntPtr.Zero;

            try
            {
                // Get PIDL for file
                if (SHParseDisplayName(filePath, IntPtr.Zero, out pidl, 0, out _) != 0 || pidl == IntPtr.Zero)
                    return;

                // Bind to parent folder
                var iidIShellFolder = typeof(IShellFolder).GUID;
                if (SHBindToParent(pidl, ref iidIShellFolder, out var parentPtr, out pidlChild) != 0)
                    return;

                var shellFolder = (IShellFolder)Marshal.GetObjectForIUnknown(parentPtr);
                var childPID = new IntPtr[] { pidlChild };
                Guid[] iids = new[] {
                    typeof(IContextMenu3).GUID,
                    typeof(IContextMenu2).GUID,
                    typeof(IContextMenu).GUID
                };

                foreach (var iid in iids)
                {
                    var tempIid = iid;
                    int hr = shellFolder.GetUIObjectOf(IntPtr.Zero, 1, ref childPID[0], ref tempIid, IntPtr.Zero, out contextMenuPtr);
                    if (hr == 0 && contextMenuPtr != IntPtr.Zero)
                        break;
                }

                if (contextMenuPtr == IntPtr.Zero) return;

                var icm = Marshal.GetTypedObjectForIUnknown(contextMenuPtr, typeof(IContextMenu3)) as IContextMenu3;
                if (icm == null) return;

                // Create menu
                hMenu = CreatePopupMenu();
                uint flags = (uint)(CMF.EXPLORE | CMF.EXTENDEDVERBS | CMF.NORMAL | CMF.INCLUDESTATIC);
                int menuResult = icm.QueryContextMenu(hMenu, 0, 1, 0x7FFF, flags);

                Console.WriteLine($"QueryContextMenu result: {menuResult}");
                if (menuResult != 0)
                {
                    var mauiApp = Microsoft.Maui.Controls.Application.Current;
                    var mauiWindow = mauiApp?.Windows.FirstOrDefault();

                    var nativeWindow = mauiWindow?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                    if (nativeWindow == null) return;

                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

                    var gch = GCHandle.Alloc(icm);
                    SetWindowSubclass(hwnd, SubclassProc, IntPtr.Zero, GCHandle.ToIntPtr(gch));
                    SetForegroundWindow(hMenu);
                    uint selectedCmd = TrackPopupMenuEx(hMenu, (uint)(TPM.LEFTALIGN | TPM.RETURNCMD | TPM.RIGHTBUTTON), screenX, screenY, hwnd, IntPtr.Zero);
                    if (selectedCmd > 0)
                    {
                        var invoke = new CMINVOKECOMMANDINFOEX();
                        invoke.cbSize = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));
                        invoke.fMask = 0;
                        invoke.hwnd = hwnd;
                        invoke.lpVerb = (IntPtr)(selectedCmd - 1); // 0-based index
                        invoke.lpVerbW = (IntPtr)(selectedCmd - 1);
                        invoke.lpParameters = IntPtr.Zero;
                        invoke.lpDirectory = IntPtr.Zero;
                        invoke.nShow = 1; // SW_SHOWNORMAL

                        // Make sure to set the struct layout properly beforehand
                        icm.InvokeCommand(ref invoke);
                    }



                    gch.Free(); // cleanup

                    Marshal.ReleaseComObject(icm);
                    Marshal.ReleaseComObject(shellFolder);
                    Marshal.Release(parentPtr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing context menu: {ex.Message}");
            }
            finally
            {
                if (hMenu != IntPtr.Zero) DestroyMenu(hMenu);
                if (pidl != IntPtr.Zero) Marshal.FreeCoTaskMem(pidl);
            }
        }

        protected static Microsoft.UI.Windowing.AppWindow GetAppWindow(MauiWinUIWindow window)
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
            return appWindow;
        }
        protected static MauiWinUIWindow? GetParentWindow()
        {
            var mauiWindow = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
            return mauiWindow?.Handler?.PlatformView as MauiWinUIWindow;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

    }
}
public static class SW
{
    public const int HIDE = 0;
    public const int SHOWNORMAL = 1;
    public const int NORMAL = 1;
    public const int SHOWMINIMIZED = 2;
    public const int SHOWMAXIMIZED = 3;
    public const int MAXIMIZE = 3;
    public const int SHOWNOACTIVATE = 4;
    public const int SHOW = 5;
    public const int MINIMIZE = 6;
    public const int SHOWMINNOACTIVE = 7;
    public const int SHOWNA = 8;
    public const int RESTORE = 9;
    public const int SHOWDEFAULT = 10;
    public const int FORCEMINIMIZE = 11;
}

public static class CMIC
{
    public const int HOTKEY = 0x00000020;
    public const int ICON = 0x00000010;
    public const int FLAG_NO_UI = 0x00000400;
    public const int UNICODE = 0x00004000;
    public const int NO_CONSOLE = 0x00008000;
    public const int ASYNCOK = 0x00100000;
    public const int NOZONECHECKS = 0x00800000;
    public const int SHIFT_DOWN = 0x10000000;
    public const int CONTROL_DOWN = 0x40000000;
    public const int FLAG_LOG_USAGE = 0x04000000;
    public const int FLAG_NOZONECHECKS = 0x00800000;
}

[Flags]
public enum CMF : uint
{
    NORMAL = 0x00000000,
    DEFAULTONLY = 0x00000001,
    VERBSONLY = 0x00000002,
    EXPLORE = 0x00000004,
    NOVERBS = 0x00000008,
    CANRENAME = 0x00000010,
    NODEFAULT = 0x00000020,
    INCLUDESTATIC = 0x00000040,
    EXTENDEDVERBS = 0x00000100,
    RESERVED = 0xffff0000
}
[Flags]
public enum TPM : uint
{
    LEFTBUTTON = 0x0000,
    RIGHTBUTTON = 0x0002,
    LEFTALIGN = 0x0000,
    CENTERALIGN = 0x0004,
    RIGHTALIGN = 0x0008,
    TOPALIGN = 0x0000,
    VCENTERALIGN = 0x0010,
    BOTTOMALIGN = 0x0020,
    RETURNCMD = 0x0100
}


#endif
