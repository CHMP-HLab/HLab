using System;
using System.Runtime.InteropServices;
using static HLab.Sys.Windows.API.CommCtrl;

namespace HLab.Sys.Windows.API;

public static partial class ShellApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFileInfo
    {
        public const int NAME_SIZE = 80;
        public nint hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public unsafe fixed char szDisplayName[260];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_SIZE)]
        public unsafe fixed char szTypeName[NAME_SIZE];
    };

    [Flags]
    public enum ShGetFileInfoFlag : uint
    {
        /// <summary>get icon</summary>
        Icon = 0x000000100,
        /// <summary>get display name</summary>
        DisplayName = 0x000000200,
        /// <summary>get type name</summary>
        TypeName = 0x000000400,
        /// <summary>get attributes</summary>
        Attributes = 0x000000800,
        /// <summary>get icon location</summary>
        IconLocation = 0x000001000,
        /// <summary>return exe type</summary>
        ExeType = 0x000002000,
        /// <summary>get system icon index</summary>
        SysIconIndex = 0x000004000,
        /// <summary>put a link overlay on icon</summary>
        LinkOverlay = 0x000008000,
        /// <summary>show icon in selected state</summary>
        Selected = 0x000010000,
        /// <summary>get only specified attributes</summary>
        Attr_Specified = 0x000020000,
        /// <summary>get large icon</summary>
        LargeIcon = 0x000000000,
        /// <summary>get small icon</summary>
        SmallIcon = 0x000000001,
        /// <summary>get open icon</summary>
        OpenIcon = 0x000000002,
        /// <summary>get shell size icon</summary>
        ShellIconSize = 0x000000004,
        /// <summary>pszPath is a pidl</summary>
        PIDL = 0x000000008,
        /// <summary>use passed dwFileAttribute</summary>
        UseFileAttributes = 0x000000010,
        /// <summary>apply the appropriate overlays</summary>
        AddOverlays = 0x000000020,
        /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
        OverlayIndex = 0x000000040,
    }

    [LibraryImport("Shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint SHGetFileInfo(
        string pszPath,
        uint dwFileAttributes,
        ref SHFileInfo psfi,
        uint cbFileInfo,
        ShGetFileInfoFlag uFlags
    );

    public enum SHImageList
    {
        Large = 0x0,
        Small = 0x1,
        ExtraLarge = 0x2,
        SysSmall = 0x3,
        Jumbo = 0x4,
        Last = 0x4,
    }

    [Flags]
    public enum ImageListDrawFlags
    {
        /// <summary>
        /// Draws the image using the background color for the image list. If the background color is the CLR_NONE value, the image is drawn transparently using the mask.
        /// </summary>
        Normal = 0x00,
        /// <summary>
        /// Draws the image transparently using the mask, regardless of the background color. This value has no effect if the image list does not contain a mask.
        /// </summary>
        Transparent = 0x1,
        /// <summary>
        /// Draws the image, blending 25 percent with the blend color specified by rgbFg. This value has no effect if the image list does not contain a mask.
        /// </summary>
        Blend25 = 0x2,
        /// <summary>
        /// Same as ILD_BLEND25.
        /// </summary>
        Focus = 0x2,
        /// <summary>
        /// Draws the image, blending 50 percent with the blend color specified by rgbFg. This value has no effect if the image list does not contain a mask.
        /// </summary>
        Blend50 = 0x4,
        /// <summary>
        /// Same as ILD_BLEND50.
        /// </summary>
        Selected = 0x4,
        /// <summary>
        /// Same as ILD_BLEND50.
        /// </summary>
        Blend = 0x4,
        /// <summary>
        /// Draws the mask.
        /// </summary>
        Mask = 0x10,
        /// <summary>
        /// If the overlay does not require a mask to be drawn, set this flag.
        /// </summary>
        Image = 0x20,
        /// <summary>
        /// Draws the image using the raster operation code specified by the dwRop member.
        /// </summary>
        Rop = 0x40,
        /// <summary>
        /// To extract the overlay image from the fStyle member, use the logical AND to combine fStyle with the ILD_OVERLAYMASK value.
        /// </summary>
        OverlayMask = 0xF00,
        /// <summary>
        /// Preserves the alpha channel in the destination.
        /// </summary>
        PreserveAlpha = 0x1000,
        /// <summary>
        /// Causes the image to be scaled to cx, cy instead of being clipped.
        /// </summary>
        Scale = 0x2000,
        /// <summary>
        /// Scales the image to the current dpi of the display.
        /// </summary>
        DpiScale = 0x4000,
        /// <summary>
        /// Windows Vista and later. Draw the image if it is available in the cache. Do not extract it automatically. The called draw method returns E_PENDING to the calling component, which should then take an alternative action, such as, provide another image and queue a background task to force the image to be loaded via ForceImagePresent using the ILFIP_ALWAYS flag. The ILD_ASYNC flag then prevents the extraction operation from blocking the current thread and is especially important if a draw method is called from the user interface (UI) thread.
        /// </summary>
        Async = 0x8000
    }


    public static Guid IImageListGuid = new("46EB5926-582E-4017-9FDF-E8998DAA0950");
    public static Guid IImageList2Guid = new("192B9D83-50FC-457B-90A0-2B82A8B5DAE1");

    [ComImport()]
    [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImageList
    {

        [PreserveSig]
        int Add(nint hbmImage, nint hbmMask, ref int pi);

        [PreserveSig]
        int ReplaceIcon(int i, nint hicon, ref int pi);

        [PreserveSig]
        int SetOverlayImage(int iImage, int iOverlay);

        [PreserveSig]
        int Replace(int i, nint hbmImage, nint hbmMask);

        [PreserveSig]
        int AddMasked(nint hbmImage, int crMask, ref int pi);

        [PreserveSig]
        int Draw(ref ImageListDrawParams pimldp);

        [PreserveSig]
        int Remove(int i);

        [PreserveSig]
        int GetIcon(int i, ImageListDrawFlags flags, ref nint picon);
        [PreserveSig]
        int GetImageInfo(int i, ref ImageInfo pImageInfo);

        [PreserveSig]
        int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);

        [PreserveSig]
        int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref nint ppv);

        [PreserveSig]
        int Clone(ref Guid riid, ref nint ppv);

        [PreserveSig]
        int GetImageRect(int i, ref WinDef.Rect prc);

        [PreserveSig]
        int GetIconSize(ref int cx, ref int cy);

        [PreserveSig]
        int SetIconSize(int cx, int cy);

        [PreserveSig]
        int GetImageCount(ref int pi);

        [PreserveSig]
        int SetImageCount(int uNewCount);

        [PreserveSig]
        int SetBkColor(int clrBk, ref int pclr);

        [PreserveSig]
        int GetBkColor(ref int pclr);

        [PreserveSig]
        int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

        [PreserveSig]
        int EndDrag();

        [PreserveSig]
        int DragEnter(nint hwndLock, int x, int y);

        [PreserveSig]
        int DragLeave(nint hwndLock);

        [PreserveSig]
        int DragMove(int x, int y);

        [PreserveSig]
        int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

        [PreserveSig]
        int DragShowNolock(
            int fShow);

        [PreserveSig]
        int GetDragImage(ref WinDef.Point ppt, ref WinDef.Point pptHotspot, ref Guid riid, ref nint ppv);

        [PreserveSig]
        int GetItemFlags(int i, ref int dwFlags);

        [PreserveSig]
        int GetOverlayImage(int iOverlay, ref int piIndex);
    };


    [DllImport("shell32.dll", EntryPoint = "#727")]
    public static extern int SHGetImageList(SHImageList iImageList, ref Guid riid, ref IImageList ppv);
}