using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214E6-0000-0000-C000-000000000046")]
public interface IShellFolder
{
    [PreserveSig]
    int ParseDisplayName(
        IntPtr hwnd,
        IntPtr pbc,
        [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
        ref uint pchEaten,
        out IntPtr ppidl,
        ref uint pdwAttributes);

    [PreserveSig]
    int EnumObjects(IntPtr hwnd, int grfFlags, out IntPtr ppenumIDList);

    [PreserveSig]
    int BindToObject(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

    [PreserveSig]
    int BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

    [PreserveSig]
    int CompareIDs(int lParam, IntPtr pidl1, IntPtr pidl2);

    [PreserveSig]
    int CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

    [PreserveSig]
    int GetAttributesOf(uint cidl, IntPtr apidl, ref uint rgfInOut);

    [PreserveSig]
    int GetUIObjectOf(IntPtr hwndOwner, uint cidl, ref IntPtr apidl, [In] ref Guid riid, IntPtr rgfReserved, out IntPtr ppv);

    [PreserveSig]
    int GetDisplayNameOf(IntPtr pidl, uint uFlags, out STRRET pName);

    [PreserveSig]
    int SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName, uint uFlags, out IntPtr ppidlOut);
}

// Helper struct
[StructLayout(LayoutKind.Explicit, Size = 520)]
public struct STRRET
{
    [FieldOffset(0)]
    public uint uType;
    [FieldOffset(4)]
    public IntPtr pOleStr;
    [FieldOffset(4)]
    public IntPtr pStr;
    [FieldOffset(4)]
    public uint uOffset;
    [FieldOffset(4)]
    public IntPtr cStr;
}
