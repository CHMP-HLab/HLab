#nullable enable
using System;
using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public enum HResult : int
{
    Ok = 0,
    False = 1,
    NoInterface = unchecked((int)0x80004002),
    NotImpl = unchecked((int)0x80004001),
    Fail = unchecked((int)0x80004005)
}

public enum DesktopWallpaperPosition
{
    Center = 0,
    Tile = 1,
    Stretch = 2,
    Fit = 3,
    Fill = 4,
    Span = 5
}

public enum DesktopSlideshowOptions
{
    ShuffleImages = 0x1
}

public enum DesktopSlideshowState
{
    Enabled = 0x1,
    Slideshow = 0x2,
    DisabledByRemoteSession = 0x4
}

public enum DesktopSlideshowDirection
{
    Forward = 0,
    Backward = 1
}

[ComImport, Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD")]
public class DesktopWallpaperClass
{
}

[ComImport]
[Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDesktopWallpaper
{

    HResult SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorId, [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);
    [PreserveSig]
    HResult GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorId, [MarshalAs(UnmanagedType.LPWStr)] ref string wallpaper);
    [PreserveSig]
    HResult GetMonitorDevicePathAt(uint monitorIndex, [MarshalAs(UnmanagedType.LPWStr)] ref string monitorId);
    [PreserveSig]
    HResult GetMonitorDevicePathCount(ref uint count);
    [PreserveSig]
    HResult GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorId, [MarshalAs(UnmanagedType.Struct)] ref WinDef.Rect displayRect);

    HResult SetBackgroundColor(uint color);

    [PreserveSig]
    HResult GetBackgroundColor(ref uint color);
    HResult SetPosition(DesktopWallpaperPosition position);

    [PreserveSig]
    HResult GetPosition(ref DesktopWallpaperPosition position);
    HResult SetSlideshow(IShellItemArray items);
    HResult GetSlideshow(ref IShellItemArray items);
    HResult SetSlideshowOptions(DesktopSlideshowOptions options, uint slideshowTick);
    [PreserveSig]
    HResult GetSlideshowOptions(out DesktopSlideshowOptions options, out uint slideshowTick);
    HResult AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorId, DesktopSlideshowDirection direction);
    HResult GetStatus(ref DesktopSlideshowState state);
    HResult Enable(bool enable);
}

public static class WindowsWallpaperHelper
{
    public struct WallpaperPerMonitorInfo
    {
        public uint Index;
        public string DevicePath;
        public WinDef.Rect Rect;
        public uint Background;
        public string FilePath;
        public DesktopWallpaperPosition Position;
    }

    public struct WallpaperInfo
    {
        public int Count;
        public uint Background;
        public DesktopWallpaperPosition Position;
    }


    public static WallpaperInfo ParseWallpapers(Action<WallpaperPerMonitorInfo> action)
    {

        var desktopWallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());

        var position = DesktopWallpaperPosition.Fill;
        if(desktopWallpaper.GetPosition(ref position)!=HResult.Ok) return new WallpaperInfo();

        uint background = 0;
        if(desktopWallpaper.GetBackgroundColor(ref background)!=HResult.Ok) return new WallpaperInfo();

        uint count = 0;

        if(desktopWallpaper.GetMonitorDevicePathCount(ref count)!=HResult.Ok) return new WallpaperInfo();
        for (uint i = 0; i < count; i++)
        {
            try
            {
                var devicePath = "";
                var filePath = "";
                WinDef.Rect rect = new();

                if (desktopWallpaper.GetMonitorDevicePathAt(i, ref devicePath)!=HResult.Ok) continue;
                if (desktopWallpaper.GetWallpaper(devicePath, ref filePath)!=HResult.Ok) continue;
                if (desktopWallpaper.GetMonitorRECT(devicePath, ref rect)!=HResult.Ok) continue;
                
                action(new WallpaperPerMonitorInfo()
                {
                    Index = i,
                    DevicePath = devicePath,
                    Rect = rect,
                    FilePath = filePath,
                    Position = position,
                    Background = background,
                });
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return new WallpaperInfo
        {
            Count = (int)count,
            Background = background, 
            Position = position
        };
    }
}

public enum ShellItemAttributeFlags
{
    And = 0x1,
    Or = 0x2,
    AppCompat = 0x3,
    Mask = 0x3,
    AllItems = 0x4000
}

[ComImport()]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
public interface IShellItemArray
{
    HResult BindToHandler(nint pbc, ref Guid bhid, ref Guid riid, ref nint ppvOut);
    HResult GetPropertyStore(GetPropertyStoreFlags flags, ref Guid riid, ref nint ppv);
    HResult GetPropertyDescriptionList(RefPropertyKey keyType, ref Guid riid, ref nint ppv);
    HResult GetAttributes(ShellItemAttributeFlags attributeFlags, int sfgaoMask, ref int psfgaoAttribs);
    HResult GetCount(ref int pdwNumItems);
    HResult GetItemAt(int dwIndex, ref IShellItem ppsi);
    HResult EnumItems(ref nint ppenumShellItems);
}

[ComImport()]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
public interface IShellItem
{
    [PreserveSig()]
    HResult BindToHandler(nint pbc, ref Guid bhid, ref Guid riid, ref nint ppv);
    HResult GetParent(ref IShellItem ppsi);
    HResult GetDisplayName(ShellItemGlobalDeviceName gdn, ref System.Text.StringBuilder name);
    HResult GetAttributes(uint sfgaoMask, ref uint psfgaoAttribs);
    HResult Compare(IShellItem psi, uint hint, ref int piOrder);
}

public enum ShellItemGlobalDeviceName : int
{
    NormalDisplay = 0x0,
    ParentRelativeParsing = unchecked((int)0x80018001),
    DesktopAbsoluteParsing = unchecked((int)0x80028000),
    ParentRelativeEditing = unchecked((int)0x80031001),
    DesktopAbsoluteEditing = unchecked((int)0x8004C000),
    FileSysPath = unchecked((int)0x80058000),
    Url = unchecked((int)0x80068000),
    ParentRelativeForAddressBar = unchecked((int)0x8007C001),
    ParentRelative = unchecked((int)0x80080001)
}

public enum GetPropertyStoreFlags
{
    Default = 0,
    HandlerPropertiesOnly = 0x1,
    ReadWrite = 0x2,
    Temporary = 0x4,
    FastPropertiesOnly = 0x8,
    OpensLowItem = 0x10,
    DelayCreation = 0x20,
    BestEffort = 0x40,
    NoOpLock = 0x80,
    PreferQueryProperties = 0x100,
    ExtrinsicProperties = 0x200,
    ExtrinsicPropertiesOnly = 0x400,
    MaskValid = 0x7FF
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct RefPropertyKey(Guid formatId, int propertyId)
{
    public Guid FormatId { get; } = formatId;
    public int PropertyId { get; } = propertyId;

    public static readonly RefPropertyKey DateCreated = new RefPropertyKey(new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC"), 15);
}