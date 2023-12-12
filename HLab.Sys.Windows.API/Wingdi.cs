using System;
using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public static partial class WinGdi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DisplayDevice
    {
        public DisplayDevice() { }
        [MarshalAs(UnmanagedType.U4)]
        public int cb = Marshal.SizeOf(typeof(DisplayDevice));
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    [Flags]
    public enum DisplayDeviceStateFlags : int
    {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VgaCompatible = 0x10,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }



    [DllImport("user32.dll", CharSet = CharSet.Unicode)]//, CallingConvention = CallingConvention.ThisCall)]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //[DllImport("user32.dll")]
    public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);

    [Flags()]
    public enum DisplayModeFlags : int
    {
        Orientation = 0x1,
        PaperSize = 0x2,
        PaperLength = 0x4,
        PaperWidth = 0x8,
        Scale = 0x10,
        Position = 0x20,
        NUP = 0x40,
        DisplayOrientation = 0x80,
        Copies = 0x100,
        DefaultSource = 0x200,
        PrintQuality = 0x400,
        Color = 0x800,
        Duplex = 0x1000,
        YResolution = 0x2000,
        TTOption = 0x4000,
        Collate = 0x8000,
        FormName = 0x10000,
        LogPixels = 0x20000,
        BitsPerPixel = 0x40000,
        PixelsWidth = 0x80000,
        PixelsHeight = 0x100000,
        DisplayFlags = 0x200000,
        DisplayFrequency = 0x400000,
        ICMMethod = 0x800000,
        ICMIntent = 0x1000000,
        MediaType = 0x2000000,
        DitherType = 0x4000000,
        PanningWidth = 0x8000000,
        PanningHeight = 0x10000000,
        DisplayFixedOutput = 0x20000000
    }








    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DevMode
    {
        public override string ToString()
        {
            return $"{PixelsWidth}x{PixelsHeight}:{DisplayFrequency}";
        }

        /// <summary>Specifies whether collation should be used when printing multiple copies.</summary>
        public enum CollateEnum : short
        {
            /// <summary>Do NOT collate when printing multiple copies.</summary>
            CollateFalse = 0,

            /// <summary>Collate when printing multiple copies.</summary>
            CollateTrue = 1
        }

        /// <summary>The printer color.</summary>
        public enum ColorEnum : short
        {
            /// <summary>Monochrome.</summary>
            ColorMonochrome = 1,

            /// <summary>Color.</summary>
            ColorColor = 2
        }

        /// <summary>How the display presents a low-resolution mode on a higher-resolution display.</summary>
        public enum DisplayFixedOutputEnum : uint
        {
            /// <summary>The display's default setting.</summary>
            Default = 0,

            /// <summary>The low-resolution image is stretched to fill the larger screen space.</summary>
            Stretch = 1,

            /// <summary>The low-resolution image is centered in the larger screen space.</summary>
            Center = 2,
        }

        /// <summary>Specifies the device's display mode.</summary>
        public enum DisplayEnum : uint
        {
            /// <summary>Specifies that the display is a noncolor device. If this flag is not set, color is assumed.</summary>
            GrayScale = 0x00000001,

            /// <summary>Specifies that the display mode is interlaced. If the flag is not set, noninterlaced is assumed.</summary>
            Interlaced = 0x00000002,

            /// <summary>Undocumented</summary>
            TextMode = 0x00000004,
        }

        /// <summary>Specifies how dithering is to be done.</summary>
        public enum DitherEnum : uint
        {
            /// <summary>No dithering.</summary>
            None = 1,

            /// <summary>Dithering with a coarse brush.</summary>
            Coarse = 2,

            /// <summary>Dithering with a fine brush.</summary>
            Fine = 3,

            /// <summary>
            /// Line art dithering, a special dithering method that produces well defined borders between black, white, and gray scaling. It
            /// is not suitable for images that include continuous graduations in intensity and hue, such as scanned photographs.
            /// </summary>
            Lineart = 4,

            /// <summary>Dithering with error diffusion.</summary>
            Errordiffusion = 5,

            /// <summary>Reserved</summary>
            Reserved6 = 6,

            /// <summary>Reserved</summary>
            Reserved7 = 7,

            /// <summary>Reserved</summary>
            Reserved8 = 8,

            /// <summary>Reserved</summary>
            Reserved9 = 9,

            /// <summary>Device does gray scaling.</summary>
            GrayScale = 10,

            /// <summary>Base for driver-defined values.</summary>
            User = 256,
        }

        /// <summary>The orientation at which images should be presented.</summary>
        public enum DisplayOrientationEnum : uint
        {
            /// <summary>The display orientation is the natural orientation of the display device; it should be used as the default.</summary>
            Default = 0,

            /// <summary>The display orientation is rotated 90 degrees (measured clockwise) from DMDO_DEFAULT.</summary>
            _90 = 1,

            /// <summary>The display orientation is rotated 180 degrees (measured clockwise) from DMDO_DEFAULT.</summary>
            _180 = 2,

            /// <summary>The display orientation is rotated 270 degrees (measured clockwise) from DMDO_DEFAULT.</summary>
            _270 = 3,
        }

        /// <summary>Selects duplex or double-sided printing for printers capable of duplex printing.</summary>
        public enum DuplexEnum : short
        {
            /// <summary>Unknown setting.</summary>
            Unknown = 0,

            /// <summary>Normal (nonduplex) printing.</summary>
            Simplex = 1,

            /// <summary>Long-edge binding, that is, the long edge of the page is vertical.</summary>
            Vertical = 2,

            /// <summary>Short-edge binding, that is, the long edge of the page is horizontal.</summary>
            Horizontal = 3,
        }




        /// <summary>
        /// Specifies which color matching method, or intent, should be used by default. This member is primarily for non-ICM applications.
        /// ICM applications can establish intents by using the ICM functions.
        /// </summary>
        public enum IcmEnum : uint
        {
            /// <summary>
            /// Color matching should optimize for color saturation. This value is the most appropriate choice for business graphs when
            /// dithering is not desired.
            /// </summary>
            Saturate = 1,

            /// <summary>
            /// Color matching should optimize for color contrast. This value is the most appropriate choice for scanned or photographic
            /// images when dithering is desired.
            /// </summary>
            Contrast = 2,

            /// <summary>
            /// Color matching should optimize to match the exact color requested. This value is most appropriate for use with business logos
            /// or other images when an exact color match is desired.
            /// </summary>
            Colorimetric = 3,

            /// <summary>
            /// Color matching should optimize to match the exact color requested without white point mapping. This value is most appropriate
            /// for use with proofing.
            /// </summary>
            AbsColorimetric = 4,

            /// <summary>Base for driver-defined values.</summary>
            User = 256,
        }



        /// <summary>
        /// Specifies how ICM is handled. For a non-ICM application, this member determines if ICM is enabled or disabled. For ICM
        /// applications, the system examines this member to determine how to handle ICM support.
        /// </summary>
        public enum IcmMethodEnum : uint
        {
            /// <summary>Specifies that ICM is disabled.</summary>
            None = 1,

            /// <summary>Specifies that ICM is handled by Windows.</summary>
            System = 2,

            /// <summary>Specifies that ICM is handled by the device driver.</summary>
            Driver = 3,

            /// <summary>Specifies that ICM is handled by the destination device.</summary>
            Device = 4,

            /// <summary>Base for driver-defined values.</summary>
            User = 256,
        }



        /// <summary>Specifies the type of media being printed on.</summary>
        public enum MediaEnum : uint
        {
            /// <summary>Plain paper.</summary>
            Standard = 1,

            /// <summary>Transparent film.</summary>
            Transparency = 2,

            /// <summary>Glossy paper.</summary>
            Glossy = 3,

            /// <summary>Base for driver-defined values.</summary>
            User = 256,
        }


        /// <summary>Specifies where the NUP is done.</summary>
        public enum NupEnum : uint
        {
            /// <summary>The print spooler does the NUP.</summary>
            System = 1,

            /// <summary>The application does the NUP.</summary>
            OneUp = 2
        }

        /// <summary>The orientation of the paper.</summary>
        public enum OrientationEnum : short
        {
            /// <summary>Portrait</summary>
            Portrait = 1,

            /// <summary>Landscape</summary>
            Landscape = 2
        }

        /// <summary>The size of the paper to print on.</summary>
        public enum PaperEnum : short
        {
            /// <summary>Letter 8 1/2 x 11 in</summary>
            Letter = 1,

            /// <summary>Letter Small 8 1/2 x 11 in</summary>
            LetterSmall = 2,

            /// <summary>Tabloid 11 x 17 in</summary>
            Tabloid = 3,

            /// <summary>Ledger 17 x 11 in</summary>
            Ledger = 4,

            /// <summary>Legal 8 1/2 x 14 in</summary>
            Legal = 5,

            /// <summary>Statement 5 1/2 x 8 1/2 in</summary>
            Statement = 6,

            /// <summary>Executive 7 1/4 x 10 1/2 in</summary>
            Executive = 7,

            /// <summary>A3 297 x 420 mm</summary>
            A3 = 8,

            /// <summary>A4 210 x 297 mm</summary>
            A4 = 9,

            /// <summary>A4 Small 210 x 297 mm</summary>
            A4Small = 10,

            /// <summary>A5 148 x 210 mm</summary>
            A5 = 11,

            /// <summary>B4 (JIS) 250 x 354</summary>
            B4 = 12,

            /// <summary>B5 (JIS) 182 x 257 mm</summary>
            B5 = 13,

            /// <summary>Folio 8 1/2 x 13 in</summary>
            Folio = 14,

            /// <summary>Quarto 215 x 275 mm</summary>
            Quarto = 15,

            /// <summary>10x14 in</summary>
            _10X14 = 16,

            /// <summary>11x17 in</summary>
            _11X17 = 17,

            /// <summary>Note 8 1/2 x 11 in</summary>
            Note = 18,

            /// <summary>Envelope #9 3 7/8 x 8 7/8</summary>
            Env9 = 19,

            /// <summary>Envelope #10 4 1/8 x 9 1/2</summary>
            Env10 = 20,

            /// <summary>Envelope #11 4 1/2 x 10 3/8</summary>
            Env11 = 21,

            /// <summary>Envelope #12 4 \276 x 11</summary>
            Env12 = 22,

            /// <summary>Envelope #14 5 x 11 1/2</summary>
            Env14 = 23,

            /// <summary>C size sheet</summary>
            CSheet = 24,

            /// <summary>D size sheet</summary>
            DSheet = 25,

            /// <summary>E size sheet</summary>
            ESheet = 26,

            /// <summary>Envelope DL 110 x 220mm</summary>
            EnvDl = 27,

            /// <summary>Envelope C5 162 x 229 mm</summary>
            EnvC5 = 28,

            /// <summary>Envelope C3 324 x 458 mm</summary>
            EnvC3 = 29,

            /// <summary>Envelope C4 229 x 324 mm</summary>
            EnvC4 = 30,

            /// <summary>Envelope C6 114 x 162 mm</summary>
            EnvC6 = 31,

            /// <summary>Envelope C65 114 x 229 mm</summary>
            EnvC65 = 32,

            /// <summary>Envelope B4 250 x 353 mm</summary>
            EnvB4 = 33,

            /// <summary>Envelope B5 176 x 250 mm</summary>
            EnvB5 = 34,

            /// <summary>Envelope B6 176 x 125 mm</summary>
            EnvB6 = 35,

            /// <summary>Envelope 110 x 230 mm</summary>
            EnvItaly = 36,

            /// <summary>Envelope Monarch 3.875 x 7.5 in</summary>
            EnvMonarch = 37,

            /// <summary>6 3/4 Envelope 3 5/8 x 6 1/2 in</summary>
            EnvPersonal = 38,

            /// <summary>US Std Fanfold 14 7/8 x 11 in</summary>
            FanfoldUs = 39,

            /// <summary>German Std Fanfold 8 1/2 x 12 in</summary>
            FanfoldStdGerman = 40,

            /// <summary>German Legal Fanfold 8 1/2 x 13 in</summary>
            FanfoldLglGerman = 41,

            /// <summary>B4 (ISO) 250 x 353 mm</summary>
            IsoB4 = 42,

            /// <summary>Japanese Postcard 100 x 148 mm</summary>
            JapanesePostcard = 43,

            /// <summary>9 x 11 in</summary>
            _9X11 = 44,

            /// <summary>10 x 11 in</summary>
            _10X11 = 45,

            /// <summary>15 x 11 in</summary>
            _15X11 = 46,

            /// <summary>Envelope Invite 220 x 220 mm</summary>
            EnvInvite = 47,

            /// <summary>RESERVED--DO NOT USE</summary>
            Reserved48 = 48,

            /// <summary>RESERVED--DO NOT USE</summary>
            Reserved49 = 49,

            /// <summary>Letter Extra 9 \275 x 12 in</summary>
            LetterExtra = 50,

            /// <summary>Legal Extra 9 \275 x 15 in</summary>
            LegalExtra = 51,

            /// <summary>Tabloid Extra 11.69 x 18 in</summary>
            TabloidExtra = 52,

            /// <summary>A4 Extra 9.27 x 12.69 in</summary>
            A4Extra = 53,

            /// <summary>Letter Transverse 8 \275 x 11 in</summary>
            LetterTransverse = 54,

            /// <summary>A4 Transverse 210 x 297 mm</summary>
            A4Transverse = 55,

            /// <summary>Letter Extra Transverse 9\275 x 12 in</summary>
            LetterExtraTransverse = 56,

            /// <summary>SuperA/SuperA/A4 227 x 356 mm</summary>
            APlus = 57,

            /// <summary>SuperB/SuperB/A3 305 x 487 mm</summary>
            BPlus = 58,

            /// <summary>Letter Plus 8.5 x 12.69 in</summary>
            LetterPlus = 59,

            /// <summary>A4 Plus 210 x 330 mm</summary>
            A4Plus = 60,

            /// <summary>A5 Transverse 148 x 210 mm</summary>
            A5Transverse = 61,

            /// <summary>B5 (JIS) Transverse 182 x 257 mm</summary>
            B5Transverse = 62,

            /// <summary>A3 Extra 322 x 445 mm</summary>
            A3Extra = 63,

            /// <summary>A5 Extra 174 x 235 mm</summary>
            A5Extra = 64,

            /// <summary>B5 (ISO) Extra 201 x 276 mm</summary>
            B5Extra = 65,

            /// <summary>A2 420 x 594 mm</summary>
            A2 = 66,

            /// <summary>A3 Transverse 297 x 420 mm</summary>
            A3Transverse = 67,

            /// <summary>A3 Extra Transverse 322 x 445 mm</summary>
            A3ExtraTransverse = 68,

            /// <summary>Japanese Double Postcard 200 x 148 mm</summary>
            DblJapanesePostcard = 69,

            /// <summary>A6 105 x 148 mm</summary>
            A6 = 70,

            /// <summary>Japanese Envelope Kaku #2</summary>
            JenvKaku2 = 71,

            /// <summary>Japanese Envelope Kaku #3</summary>
            JenvKaku3 = 72,

            /// <summary>Japanese Envelope Chou #3</summary>
            JenvChou3 = 73,

            /// <summary>Japanese Envelope Chou #4</summary>
            JenvChou4 = 74,

            /// <summary>Letter Rotated 11 x 8 1/2 11 in</summary>
            LetterRotated = 75,

            /// <summary>A3 Rotated 420 x 297 mm</summary>
            A3Rotated = 76,

            /// <summary>A4 Rotated 297 x 210 mm</summary>
            A4Rotated = 77,

            /// <summary>A5 Rotated 210 x 148 mm</summary>
            A5Rotated = 78,

            /// <summary>B4 (JIS) Rotated 364 x 257 mm</summary>
            B4JisRotated = 79,

            /// <summary>B5 (JIS) Rotated 257 x 182 mm</summary>
            B5JisRotated = 80,

            /// <summary>Japanese Postcard Rotated 148 x 100 mm</summary>
            JapanesePostcardRotated = 81,

            /// <summary>Double Japanese Postcard Rotated 148 x 200 mm</summary>
            DblJapanesePostcardRotated = 82,

            /// <summary>A6 Rotated 148 x 105 mm</summary>
            A6Rotated = 83,

            /// <summary>Japanese Envelope Kaku #2 Rotated</summary>
            JenvKaku2Rotated = 84,

            /// <summary>Japanese Envelope Kaku #3 Rotated</summary>
            JenvKaku3Rotated = 85,

            /// <summary>Japanese Envelope Chou #3 Rotated</summary>
            JenvChou3Rotated = 86,

            /// <summary>Japanese Envelope Chou #4 Rotated</summary>
            JenvChou4Rotated = 87,

            /// <summary>B6 (JIS) 128 x 182 mm</summary>
            B6Jis = 88,

            /// <summary>B6 (JIS) Rotated 182 x 128 mm</summary>
            B6JisRotated = 89,

            /// <summary>12 x 11 in</summary>
            _12X11 = 90,

            /// <summary>Japanese Envelope You #4</summary>
            JenvYou4 = 91,

            /// <summary>Japanese Envelope You #4 Rotated</summary>
            JenvYou4Rotated = 92,

            /// <summary>PRC 16K 146 x 215 mm</summary>
            P16K = 93,

            /// <summary>PRC 32K 97 x 151 mm</summary>
            P32K = 94,

            /// <summary>PRC 32K(Big) 97 x 151 mm</summary>
            P32Kbig = 95,

            /// <summary>PRC Envelope #1 102 x 165 mm</summary>
            Penv1 = 96,

            /// <summary>PRC Envelope #2 102 x 176 mm</summary>
            Penv2 = 97,

            /// <summary>PRC Envelope #3 125 x 176 mm</summary>
            Penv3 = 98,

            /// <summary>PRC Envelope #4 110 x 208 mm</summary>
            Penv4 = 99,

            /// <summary>PRC Envelope #5 110 x 220 mm</summary>
            Penv5 = 100,

            /// <summary>PRC Envelope #6 120 x 230 mm</summary>
            Penv6 = 101,

            /// <summary>PRC Envelope #7 160 x 230 mm</summary>
            Penv7 = 102,

            /// <summary>PRC Envelope #8 120 x 309 mm</summary>
            Penv8 = 103,

            /// <summary>PRC Envelope #9 229 x 324 mm</summary>
            Penv9 = 104,

            /// <summary>PRC Envelope #10 324 x 458 mm</summary>
            Penv10 = 105,

            /// <summary>PRC 16K Rotated</summary>
            P16KRotated = 106,

            /// <summary>PRC 32K Rotated</summary>
            P32KRotated = 107,

            /// <summary>PRC 32K(Big) Rotated</summary>
            P32KbigRotated = 108,

            /// <summary>PRC Envelope #1 Rotated 165 x 102 mm</summary>
            Penv1Rotated = 109,

            /// <summary>PRC Envelope #2 Rotated 176 x 102 mm</summary>
            Penv2Rotated = 110,

            /// <summary>PRC Envelope #3 Rotated 176 x 125 mm</summary>
            Penv3Rotated = 111,

            /// <summary>PRC Envelope #4 Rotated 208 x 110 mm</summary>
            Penv4Rotated = 112,

            /// <summary>PRC Envelope #5 Rotated 220 x 110 mm</summary>
            Penv5Rotated = 113,

            /// <summary>PRC Envelope #6 Rotated 230 x 120 mm</summary>
            Penv6Rotated = 114,

            /// <summary>PRC Envelope #7 Rotated 230 x 160 mm</summary>
            Penv7Rotated = 115,

            /// <summary>PRC Envelope #8 Rotated 309 x 120 mm</summary>
            Penv8Rotated = 116,

            /// <summary>PRC Envelope #9 Rotated 324 x 229 mm</summary>
            Penv9Rotated = 117,

            /// <summary>PRC Envelope #10 Rotated 458 x 324 mm</summary>
            Penv10Rotated = 118,

            /// <summary>User-defined lower bounds.</summary>
            User = 256,
        }


        /// <summary>The printer resolution.</summary>
        public enum ResolutionEnum : short
        {
            /// <summary>Use draft resolution (96 DPI).</summary>
            Draft = -1,

            /// <summary>Use low resolution (150 DPI).</summary>
            Low = -2,

            /// <summary>Use medium resolution (300 DPI).</summary>
            Medium = -3,

            /// <summary>Use high resolution (600 DPI).</summary>
            High = -4,
        }

        /// <summary>Specifies how TrueType fonts should be printed.</summary>
        public enum TTEnum : short
        {
            /// <summary>Prints TrueType fonts as graphics. This is the default action for dot-matrix printers.</summary>
            Bitmap = 1,

            /// <summary>
            /// Downloads TrueType fonts as soft fonts. This is the default action for Hewlett-Packard printers that use Printer Control
            /// Language (PCL).
            /// </summary>
            Download = 2,

            /// <summary>Substitute device fonts for TrueType fonts. This is the default action for PostScript printers.</summary>
            SubstituteDevice = 3,

            /// <summary>Downloads TrueType fonts as outline soft fonts.</summary>
            DownloadOutline = 4
        }

        public enum SpecVersionEnum : ushort
        {
            NT4 = 0x0400,
            Win2K = 0x0500,
            WinXP = 0x0501,
            WS03 = 0x0502,
            Vista = 0x0600,
            Win7 = 0x0601,
            Win8 = 0x0602
        };

        /// <summary>
        /// A zero-terminated character array that specifies the "friendly" name of the printer or display; for example, "PCL/HP
        /// LaserJet" in the case of PCL/HP LaserJet. This string is unique among device drivers. Note that this name may be truncated to
        /// fit in the <c>dmDeviceName</c> array.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        /// <summary>
        /// The version number of the initialization data specification on which the structure is based. To ensure the correct version is
        /// used for any operating system, use DM_SPECVERSION.
        /// </summary>
        public SpecVersionEnum SpecVersion;

        /// <summary>The driver version number assigned by the driver developer.</summary>
        public ushort DriverVersion;

        /// <summary>
        /// Specifies the size, in bytes, of the <c>DEVMODE</c> structure, not including any private driver-specific data that might
        /// follow the structure's public members. Set this member to to indicate the version of the <c>DEVMODE</c> structure being used.
        /// </summary>
        public ushort Size;

        /// <summary>
        /// Contains the number of bytes of private driver-data that follow this structure. If a device driver does not use
        /// device-specific information, set this member to zero.
        /// </summary>
        public ushort DriverExtra;

        /// <summary>
        /// <para>
        /// Specifies whether certain members of the <c>DEVMODE</c> structure have been initialized. If a member is initialized, its
        /// corresponding bit is set, otherwise the bit is clear. A driver supports only those <c>DEVMODE</c> members that are
        /// appropriate for the printer or display technology.
        /// </para>
        /// <para>The following values are defined, and are listed here with the corresponding structure members.</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Structure member</term>
        /// </listheader>
        /// <item>
        /// <term>DM_ORIENTATION</term>
        /// <term>dmOrientation</term>
        /// </item>
        /// <item>
        /// <term>DM_PAPERSIZE</term>
        /// <term>dmPaperSize</term>
        /// </item>
        /// <item>
        /// <term>DM_PAPERLENGTH</term>
        /// <term>dmPaperLength</term>
        /// </item>
        /// <item>
        /// <term>DM_PAPERWIDTH</term>
        /// <term>dmPaperWidth</term>
        /// </item>
        /// <item>
        /// <term>DM_SCALE</term>
        /// <term>dmScale</term>
        /// </item>
        /// <item>
        /// <term>DM_COPIES</term>
        /// <term>dmCopies</term>
        /// </item>
        /// <item>
        /// <term>DM_DEFAULTSOURCE</term>
        /// <term>dmDefaultSource</term>
        /// </item>
        /// <item>
        /// <term>DM_PRINTQUALITY</term>
        /// <term>dmPrintQuality</term>
        /// </item>
        /// <item>
        /// <term>DM_POSITION</term>
        /// <term>dmPosition</term>
        /// </item>
        /// <item>
        /// <term>DM_DISPLAYORIENTATION</term>
        /// <term>dmDisplayOrientation</term>
        /// </item>
        /// <item>
        /// <term>DM_DISPLAYFIXEDOUTPUT</term>
        /// <term>dmDisplayFixedOutput</term>
        /// </item>
        /// <item>
        /// <term>DM_COLOR</term>
        /// <term>dmColor</term>
        /// </item>
        /// <item>
        /// <term>DM_DUPLEX</term>
        /// <term>dmDuplex</term>
        /// </item>
        /// <item>
        /// <term>DM_YRESOLUTION</term>
        /// <term>dmYResolution</term>
        /// </item>
        /// <item>
        /// <term>DM_TTOPTION</term>
        /// <term>dmTTOption</term>
        /// </item>
        /// <item>
        /// <term>DM_COLLATE</term>
        /// <term>dmCollate</term>
        /// </item>
        /// <item>
        /// <term>DM_FORMNAME</term>
        /// <term>dmFormName</term>
        /// </item>
        /// <item>
        /// <term>DM_LOGPIXELS</term>
        /// <term>dmLogPixels</term>
        /// </item>
        /// <item>
        /// <term>DM_BITSPERPEL</term>
        /// <term>dmBitsPerPel</term>
        /// </item>
        /// <item>
        /// <term>DM_PELSWIDTH</term>
        /// <term>dmPelsWidth</term>
        /// </item>
        /// <item>
        /// <term>DM_PELSHEIGHT</term>
        /// <term>dmPelsHeight</term>
        /// </item>
        /// <item>
        /// <term>DM_DISPLAYFLAGS</term>
        /// <term>dmDisplayFlags</term>
        /// </item>
        /// <item>
        /// <term>DM_NUP</term>
        /// <term>dmNup</term>
        /// </item>
        /// <item>
        /// <term>DM_DISPLAYFREQUENCY</term>
        /// <term>dmDisplayFrequency</term>
        /// </item>
        /// <item>
        /// <term>DM_ICMMETHOD</term>
        /// <term>dmICMMethod</term>
        /// </item>
        /// <item>
        /// <term>DM_ICMINTENT</term>
        /// <term>dmICMIntent</term>
        /// </item>
        /// <item>
        /// <term>DM_MEDIATYPE</term>
        /// <term>dmMediaType</term>
        /// </item>
        /// <item>
        /// <term>DM_DITHERTYPE</term>
        /// <term>dmDitherType</term>
        /// </item>
        /// <item>
        /// <term>DM_PANNINGWIDTH</term>
        /// <term>dmPanningWidth</term>
        /// </item>
        /// <item>
        /// <term>DM_PANNINGHEIGHT</term>
        /// <term>dmPanningHeight</term>
        /// </item>
        /// </list>
        /// </summary>
		public DisplayModeFlags Fields;

        /// <summary>DUMMYUNIONNAME</summary>
        DevModeU1 Union;

        [StructLayout(LayoutKind.Explicit)]
        struct DevModeU1
        {
            [FieldOffset(0)]
            public OrientationEnum Orientation;

            [FieldOffset(2)]
            public PaperEnum PaperSize;

            [FieldOffset(4)]
            public short PaperLength;

            [FieldOffset(6)]
            public short PaperWidth;

            [FieldOffset(8)]
            public short Scale;

            [FieldOffset(10)]
            public short Copies;

            [FieldOffset(12)]
            public short DefaultSource;

            [FieldOffset(14)]
            public ResolutionEnum PrintQuality;

            [FieldOffset(0)]
            public WinDef.Point Position;

            [FieldOffset(8)]
            public DisplayOrientationEnum DisplayOrientation;

            [FieldOffset(12)]
            public DisplayFixedOutputEnum DisplayFixedOutput;
        }

        /// <summary>
        /// For printer devices only, selects the orientation of the paper. This member can be either DMORIENT_PORTRAIT (1) or
        /// DMORIENT_LANDSCAPE (2).
        /// </summary>
        public OrientationEnum Orientation { get => Union.Orientation; set => Union.Orientation = value; }

        /// <summary>
        /// <para>
        /// For printer devices only, selects the size of the paper to print on. This member can be set to zero if the length and width
        /// of the paper are both set by the <c>dmPaperLength</c> and <c>dmPaperWidth</c> members. Otherwise, the <c>dmPaperSize</c>
        /// member can be set to a device specific value greater than or equal to USER or to one of the following predefined values.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>LETTER</term>
        /// <term>Letter, 8 1/2- by 11-inches</term>
        /// </item>
        /// <item>
        /// <term>LEGAL</term>
        /// <term>Legal, 8 1/2- by 14-inches</term>
        /// </item>
        /// <item>
        /// <term>9X11</term>
        /// <term>9- by 11-inch sheet</term>
        /// </item>
        /// <item>
        /// <term>10X11</term>
        /// <term>10- by 11-inch sheet</term>
        /// </item>
        /// <item>
        /// <term>10X14</term>
        /// <term>10- by 14-inch sheet</term>
        /// </item>
        /// <item>
        /// <term>15X11</term>
        /// <term>15- by 11-inch sheet</term>
        /// </item>
        /// <item>
        /// <term>11X17</term>
        /// <term>11- by 17-inch sheet</term>
        /// </item>
        /// <item>
        /// <term>12X11</term>
        /// <term>12- by 11-inch sheet</term>
        /// </item>
        /// <item>
        /// <term>A2</term>
        /// <term>A2 sheet, 420 x 594-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A3</term>
        /// <term>A3 sheet, 297- by 420-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A3_EXTRA</term>
        /// <term>A3 Extra 322 x 445-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A3_EXTRA_TRAVERSE</term>
        /// <term>A3 Extra Transverse 322 x 445-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A3_ROTATED</term>
        /// <term>A3 rotated sheet, 420- by 297-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A3_TRAVERSE</term>
        /// <term>A3 Transverse 297 x 420-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A4</term>
        /// <term>A4 sheet, 210- by 297-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A4_EXTRA</term>
        /// <term>A4 sheet, 9.27 x 12.69 inches</term>
        /// </item>
        /// <item>
        /// <term>A4_PLUS</term>
        /// <term>A4 Plus 210 x 330-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A4_ROTATED</term>
        /// <term>A4 rotated sheet, 297- by 210-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A4SMALL</term>
        /// <term>A4 small sheet, 210- by 297-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A4_TRANSVERSE</term>
        /// <term>A4 Transverse 210 x 297 millimeters</term>
        /// </item>
        /// <item>
        /// <term>A5</term>
        /// <term>A5 sheet, 148- by 210-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A5_EXTRA</term>
        /// <term>A5 Extra 174 x 235-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A5_ROTATED</term>
        /// <term>A5 rotated sheet, 210- by 148-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A5_TRANSVERSE</term>
        /// <term>A5 Transverse 148 x 210-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A6</term>
        /// <term>A6 sheet, 105- by 148-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A6_ROTATED</term>
        /// <term>A6 rotated sheet, 148- by 105-millimeters</term>
        /// </item>
        /// <item>
        /// <term>A_PLUS</term>
        /// <term>SuperA/A4 227 x 356 -millimeters</term>
        /// </item>
        /// <item>
        /// <term>B4</term>
        /// <term>B4 sheet, 250- by 354-millimeters</term>
        /// </item>
        /// <item>
        /// <term>B4_JIS_ROTATED</term>
        /// <term>B4 (JIS) rotated sheet, 364- by 257-millimeters</term>
        /// </item>
        /// <item>
        /// <term>B5</term>
        /// <term>B5 sheet, 182- by 257-millimeter paper</term>
        /// </item>
        /// <item>
        /// <term>B5_EXTRA</term>
        /// <term>B5 (ISO) Extra 201 x 276-millimeters</term>
        /// </item>
        /// <item>
        /// <term>B5_JIS_ROTATED</term>
        /// <term>B5 (JIS) rotated sheet, 257- by 182-millimeters</term>
        /// </item>
        /// <item>
        /// <term>B6_JIS</term>
        /// <term>B6 (JIS) sheet, 128- by 182-millimeters</term>
        /// </item>
        /// <item>
        /// <term>B6_JIS_ROTATED</term>
        /// <term>B6 (JIS) rotated sheet, 182- by 128-millimeters</term>
        /// </item>
        /// <item>
        /// <term>B_PLUS</term>
        /// <term>SuperB/A3 305 x 487-millimeters</term>
        /// </item>
        /// <item>
        /// <term>CSHEET</term>
        /// <term>C Sheet, 17- by 22-inches</term>
        /// </item>
        /// <item>
        /// <term>DBL_JAPANESE_POSTCARD</term>
        /// <term>Double Japanese Postcard, 200- by 148-millimeters</term>
        /// </item>
        /// <item>
        /// <term>DBL_JAPANESE_POSTCARD_ROTATED</term>
        /// <term>Double Japanese Postcard Rotated, 148- by 200-millimeters</term>
        /// </item>
        /// <item>
        /// <term>DSHEET</term>
        /// <term>D Sheet, 22- by 34-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_9</term>
        /// <term>#9 Envelope, 3 7/8- by 8 7/8-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_10</term>
        /// <term>#10 Envelope, 4 1/8- by 9 1/2-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_11</term>
        /// <term>#11 Envelope, 4 1/2- by 10 3/8-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_12</term>
        /// <term>#12 Envelope, 4 3/4- by 11-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_14</term>
        /// <term>#14 Envelope, 5- by 11 1/2-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_C5</term>
        /// <term>C5 Envelope, 162- by 229-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_C3</term>
        /// <term>C3 Envelope, 324- by 458-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_C4</term>
        /// <term>C4 Envelope, 229- by 324-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_C6</term>
        /// <term>C6 Envelope, 114- by 162-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_C65</term>
        /// <term>C65 Envelope, 114- by 229-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_B4</term>
        /// <term>B4 Envelope, 250- by 353-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_B5</term>
        /// <term>B5 Envelope, 176- by 250-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_B6</term>
        /// <term>B6 Envelope, 176- by 125-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_DL</term>
        /// <term>DL Envelope, 110- by 220-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_INVITE</term>
        /// <term>Envelope Invite 220 x 220 mm</term>
        /// </item>
        /// <item>
        /// <term>ENV_ITALY</term>
        /// <term>Italy Envelope, 110- by 230-millimeters</term>
        /// </item>
        /// <item>
        /// <term>ENV_MONARCH</term>
        /// <term>Monarch Envelope, 3 7/8- by 7 1/2-inches</term>
        /// </item>
        /// <item>
        /// <term>ENV_PERSONAL</term>
        /// <term>6 3/4 Envelope, 3 5/8- by 6 1/2-inches</term>
        /// </item>
        /// <item>
        /// <term>ESHEET</term>
        /// <term>E Sheet, 34- by 44-inches</term>
        /// </item>
        /// <item>
        /// <term>EXECUTIVE</term>
        /// <term>Executive, 7 1/4- by 10 1/2-inches</term>
        /// </item>
        /// <item>
        /// <term>FANFOLD_US</term>
        /// <term>US Std Fanfold, 14 7/8- by 11-inches</term>
        /// </item>
        /// <item>
        /// <term>FANFOLD_STD_GERMAN</term>
        /// <term>German Std Fanfold, 8 1/2- by 12-inches</term>
        /// </item>
        /// <item>
        /// <term>FANFOLD_LGL_GERMAN</term>
        /// <term>German Legal Fanfold, 8 - by 13-inches</term>
        /// </item>
        /// <item>
        /// <term>FOLIO</term>
        /// <term>Folio, 8 1/2- by 13-inch paper</term>
        /// </item>
        /// <item>
        /// <term>ISO_B4</term>
        /// <term>B4 (ISO) 250- by 353-millimeters paper</term>
        /// </item>
        /// <item>
        /// <term>JAPANESE_POSTCARD</term>
        /// <term>Japanese Postcard, 100- by 148-millimeters</term>
        /// </item>
        /// <item>
        /// <term>JAPANESE_POSTCARD_ROTATED</term>
        /// <term>Japanese Postcard Rotated, 148- by 100-millimeters</term>
        /// </item>
        /// <item>
        /// <term>JENV_CHOU3</term>
        /// <term>Japanese Envelope Chou #3</term>
        /// </item>
        /// <item>
        /// <term>JENV_CHOU3_ROTATED</term>
        /// <term>Japanese Envelope Chou #3 Rotated</term>
        /// </item>
        /// <item>
        /// <term>JENV_CHOU4</term>
        /// <term>Japanese Envelope Chou #4</term>
        /// </item>
        /// <item>
        /// <term>JENV_CHOU4_ROTATED</term>
        /// <term>Japanese Envelope Chou #4 Rotated</term>
        /// </item>
        /// <item>
        /// <term>JENV_KAKU2</term>
        /// <term>Japanese Envelope Kaku #2</term>
        /// </item>
        /// <item>
        /// <term>JENV_KAKU2_ROTATED</term>
        /// <term>Japanese Envelope Kaku #2 Rotated</term>
        /// </item>
        /// <item>
        /// <term>JENV_KAKU3</term>
        /// <term>Japanese Envelope Kaku #3</term>
        /// </item>
        /// <item>
        /// <term>JENV_KAKU3_ROTATED</term>
        /// <term>Japanese Envelope Kaku #3 Rotated</term>
        /// </item>
        /// <item>
        /// <term>JENV_YOU4</term>
        /// <term>Japanese Envelope You #4</term>
        /// </item>
        /// <item>
        /// <term>JENV_YOU4_ROTATED</term>
        /// <term>Japanese Envelope You #4 Rotated</term>
        /// </item>
        /// <item>
        /// <term>LAST</term>
        /// <term>PENV_10_ROTATED</term>
        /// </item>
        /// <item>
        /// <term>LEDGER</term>
        /// <term>Ledger, 17- by 11-inches</term>
        /// </item>
        /// <item>
        /// <term>LEGAL_EXTRA</term>
        /// <term>Legal Extra 9 1/2 x 15 inches.</term>
        /// </item>
        /// <item>
        /// <term>LETTER_EXTRA</term>
        /// <term>Letter Extra 9 1/2 x 12 inches.</term>
        /// </item>
        /// <item>
        /// <term>LETTER_EXTRA_TRANSVERSE</term>
        /// <term>Letter Extra Transverse 9 1/2 x 12 inches.</term>
        /// </item>
        /// <item>
        /// <term>LETTER_ROTATED</term>
        /// <term>Letter Rotated 11 by 8 1/2 inches</term>
        /// </item>
        /// <item>
        /// <term>LETTERSMALL</term>
        /// <term>Letter Small, 8 1/2- by 11-inches</term>
        /// </item>
        /// <item>
        /// <term>LETTER_TRANSVERSE</term>
        /// <term>Letter Transverse 8 1/2 x 11-inches</term>
        /// </item>
        /// <item>
        /// <term>NOTE</term>
        /// <term>Note, 8 1/2- by 11-inches</term>
        /// </item>
        /// <item>
        /// <term>P16K</term>
        /// <term>PRC 16K, 146- by 215-millimeters</term>
        /// </item>
        /// <item>
        /// <term>P16K_ROTATED</term>
        /// <term>PRC 16K Rotated, 215- by 146-millimeters</term>
        /// </item>
        /// <item>
        /// <term>P32K</term>
        /// <term>PRC 32K, 97- by 151-millimeters</term>
        /// </item>
        /// <item>
        /// <term>P32K_ROTATED</term>
        /// <term>PRC 32K Rotated, 151- by 97-millimeters</term>
        /// </item>
        /// <item>
        /// <term>P32KBIG</term>
        /// <term>PRC 32K(Big) 97- by 151-millimeters</term>
        /// </item>
        /// <item>
        /// <term>P32KBIG_ROTATED</term>
        /// <term>PRC 32K(Big) Rotated, 151- by 97-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_1</term>
        /// <term>PRC Envelope #1, 102- by 165-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_1_ROTATED</term>
        /// <term>PRC Envelope #1 Rotated, 165- by 102-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_2</term>
        /// <term>PRC Envelope #2, 102- by 176-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_2_ROTATED</term>
        /// <term>PRC Envelope #2 Rotated, 176- by 102-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_3</term>
        /// <term>PRC Envelope #3, 125- by 176-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_3_ROTATED</term>
        /// <term>PRC Envelope #3 Rotated, 176- by 125-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_4</term>
        /// <term>PRC Envelope #4, 110- by 208-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_4_ROTATED</term>
        /// <term>PRC Envelope #4 Rotated, 208- by 110-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_5</term>
        /// <term>PRC Envelope #5, 110- by 220-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_5_ROTATED</term>
        /// <term>PRC Envelope #5 Rotated, 220- by 110-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_6</term>
        /// <term>PRC Envelope #6, 120- by 230-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_6_ROTATED</term>
        /// <term>PRC Envelope #6 Rotated, 230- by 120-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_7</term>
        /// <term>PRC Envelope #7, 160- by 230-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_7_ROTATED</term>
        /// <term>PRC Envelope #7 Rotated, 230- by 160-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_8</term>
        /// <term>PRC Envelope #8, 120- by 309-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_8_ROTATED</term>
        /// <term>PRC Envelope #8 Rotated, 309- by 120-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_9</term>
        /// <term>PRC Envelope #9, 229- by 324-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_9_ROTATED</term>
        /// <term>PRC Envelope #9 Rotated, 324- by 229-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_10</term>
        /// <term>PRC Envelope #10, 324- by 458-millimeters</term>
        /// </item>
        /// <item>
        /// <term>PENV_10_ROTATED</term>
        /// <term>PRC Envelope #10 Rotated, 458- by 324-millimeters</term>
        /// </item>
        /// <item>
        /// <term>QUARTO</term>
        /// <term>Quarto, 215- by 275-millimeter paper</term>
        /// </item>
        /// <item>
        /// <term>STATEMENT</term>
        /// <term>Statement, 5 1/2- by 8 1/2-inches</term>
        /// </item>
        /// <item>
        /// <term>TABLOID</term>
        /// <term>Tabloid, 11- by 17-inches</term>
        /// </item>
        /// <item>
        /// <term>TABLOID_EXTRA</term>
        /// <term>Tabloid, 11.69 x 18-inches</term>
        /// </item>
        /// </list>
        /// </summary>
        public PaperEnum PaperSize { get => Union.PaperSize; set => Union.PaperSize = value; }

        /// <summary>
        /// For printer devices only, overrides the length of the paper specified by the <c>dmPaperSize</c> member, either for custom
        /// paper sizes or for devices such as dot-matrix printers that can print on a page of arbitrary length. These values, along with
        /// all other values in this structure that specify a physical length, are in tenths of a millimeter.
        /// </summary>
        public short PaperLength { get => Union.PaperLength; set => Union.PaperLength = value; }

        /// <summary>For printer devices only, overrides the width of the paper specified by the <c>dmPaperSize</c> member.</summary>
        public short PaperWidth { get => Union.PaperWidth; set => Union.PaperWidth = value; }

        /// <summary>
        /// Specifies the factor by which the printed output is to be scaled. The apparent page size is scaled from the physical page
        /// size by a factor of <c>dmScale</c> /100. For example, a letter-sized page with a <c>dmScale</c> value of 50 would contain as
        /// much data as a page of 17- by 22-inches because the output text and graphics would be half their original height and width.
        /// </summary>
        public short Scale { get => Union.Scale; set => Union.Scale = value; }

        /// <summary>Selects the number of copies printed if the device supports multiple-page copies.</summary>
        public short Copies { get => Union.Copies; set => Union.Copies = value; }

        /// <summary>
        /// <para>
        /// Specifies the paper source. To retrieve a list of the available paper sources for a printer, use the DeviceCapabilities
        /// function with the DC_BINS flag.
        /// </para>
        /// <para>This member can be one of the following values, or it can be a device-specific value greater than or equal to DMBIN_USER.</para>
        /// </summary>
		public short DefaultSource { get => Union.DefaultSource; set => Union.DefaultSource = value; }

        /// <summary>
        /// <para>Specifies the printer resolution. There are four predefined device-independent values:</para>
        /// <para>If a positive value is specified, it specifies the number of dots per inch (DPI) and is therefore device dependent.</para>
        /// </summary>
        public ResolutionEnum PrintQuality { get => Union.PrintQuality; set => Union.PrintQuality = value; }

        /// <summary>
        /// For display devices only, a POINTL structure that indicates the positional coordinates of the display device in reference to
        /// the desktop area. The primary display device is always located at coordinates (0,0).
        /// </summary>
        public WinDef.Point Position { get => Union.Position; set => Union.Position = value; }

        /// <summary>
        /// <para>
        /// For display devices only, the orientation at which images should be presented. If DM_DISPLAYORIENTATION is not set, this
        /// member must be zero. If DM_DISPLAYORIENTATION is set, this member must be one of the following values
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMDO_DEFAULT</term>
        /// <term>The display orientation is the natural orientation of the display device; it should be used as the default.</term>
        /// </item>
        /// <item>
        /// <term>DMDO_90</term>
        /// <term>The display orientation is rotated 90 degrees (measured clockwise) from DMDO_DEFAULT.</term>
        /// </item>
        /// <item>
        /// <term>DMDO_180</term>
        /// <term>The display orientation is rotated 180 degrees (measured clockwise) from DMDO_DEFAULT.</term>
        /// </item>
        /// <item>
        /// <term>DMDO_270</term>
        /// <term>The display orientation is rotated 270 degrees (measured clockwise) from DMDO_DEFAULT.</term>
        /// </item>
        /// </list>
        /// <para>
        /// To determine whether the display orientation is portrait or landscape orientation, check the ratio of <c>dmPelsWidth</c> to <c>dmPelsHeight</c>.
        /// </para>
        /// <para><c>Windows 2000:</c> Not supported.</para>
        /// </summary>
        public DisplayOrientationEnum DisplayOrientation { get => Union.DisplayOrientation; set => Union.DisplayOrientation = value; }

        /// <summary>
        /// <para>
        /// For fixed-resolution display devices only, how the display presents a low-resolution mode on a higher-resolution display. For
        /// example, if a display device's resolution is fixed at 1024 x 768 pixels but its mode is set to 640 x 480 pixels, the device
        /// can either display a 640 x 480 image somewhere in the interior of the 1024 x 768 screen space or stretch the 640 x 480 image
        /// to fill the larger screen space. If DM_DISPLAYFIXEDOUTPUT is not set, this member must be zero. If DM_DISPLAYFIXEDOUTPUT is
        /// set, this member must be one of the following values.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMDFO_DEFAULT</term>
        /// <term>The display's default setting.</term>
        /// </item>
        /// <item>
        /// <term>DMDFO_CENTER</term>
        /// <term>The low-resolution image is centered in the larger screen space.</term>
        /// </item>
        /// <item>
        /// <term>DMDFO_STRETCH</term>
        /// <term>The low-resolution image is stretched to fill the larger screen space.</term>
        /// </item>
        /// </list>
        /// <para><c>Windows 2000:</c> Not supported.</para>
        /// </summary>
		public DisplayFixedOutputEnum DisplayFixedOutput { get => Union.DisplayFixedOutput; set => Union.DisplayFixedOutput = value; }

        /// <summary>
        /// <para>Switches between color and monochrome on color printers. The following are the possible values:</para>
        /// <list type="bullet">
        /// <item>
        /// <term>DMCOLOR_COLOR</term>
        /// </item>
        /// <item>
        /// <term>DMCOLOR_MONOCHROME</term>
        /// </item>
        /// </list>
        /// </summary>
        public ColorEnum Color;

        /// <summary>
        /// <para>Selects duplex or double-sided printing for printers capable of duplex printing. Following are the possible values.</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMDUP_SIMPLEX</term>
        /// <term>Normal (nonduplex) printing.</term>
        /// </item>
        /// <item>
        /// <term>DMDUP_HORIZONTAL</term>
        /// <term>Short-edge binding, that is, the long edge of the page is horizontal.</term>
        /// </item>
        /// <item>
        /// <term>DMDUP_VERTICAL</term>
        /// <term>Long-edge binding, that is, the long edge of the page is vertical.</term>
        /// </item>
        /// </list>
        /// </summary>
        public DuplexEnum Duplex;

        /// <summary>
        /// Specifies the y-resolution, in dots per inch, of the printer. If the printer initializes this member, the
        /// <c>dmPrintQuality</c> member specifies the x-resolution, in dots per inch, of the printer.
        /// </summary>
        public short YResolution;

        /// <summary>
        /// <para>Specifies how TrueType fonts should be printed. This member can be one of the following values.</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMTT_BITMAP</term>
        /// <term>Prints TrueType fonts as graphics. This is the default action for dot-matrix printers.</term>
        /// </item>
        /// <item>
        /// <term>DMTT_DOWNLOAD</term>
        /// <term>
        /// Downloads TrueType fonts as soft fonts. This is the default action for Hewlett-Packard printers that use Printer Control
        /// Language (PCL).
        /// </term>
        /// </item>
        /// <item>
        /// <term>DMTT_DOWNLOAD_OUTLINE</term>
        /// <term>Downloads TrueType fonts as outline soft fonts.</term>
        /// </item>
        /// <item>
        /// <term>DMTT_SUBDEV</term>
        /// <term>Substitutes device fonts for TrueType fonts. This is the default action for PostScript printers.</term>
        /// </item>
        /// </list>
        /// </summary>
        public TTEnum TTOption;

        /// <summary>
        /// <para>
        /// Specifies whether collation should be used when printing multiple copies. (This member is ignored unless the printer driver
        /// indicates support for collation by setting the <c>dmFields</c> member to DM_COLLATE.) This member can be one of the following values.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMCOLLATE_TRUE</term>
        /// <term>Collate when printing multiple copies.</term>
        /// </item>
        /// <item>
        /// <term>DMCOLLATE_FALSE</term>
        /// <term>Do not collate when printing multiple copies.</term>
        /// </item>
        /// </list>
        /// </summary>
        public CollateEnum Collate;

        /// <summary>
        /// A zero-terminated character array that specifies the name of the form to use; for example, "Letter" or "Legal". A complete
        /// set of names can be retrieved by using the EnumForms function.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FormName;

        /// <summary>The number of pixels per logical inch. Printer drivers do not use this member.</summary>
        public ushort LogPixels;

        /// <summary>
        /// Specifies the color resolution, in bits per pixel, of the display device (for example: 4 bits for 16 colors, 8 bits for 256
        /// colors, or 16 bits for 65,536 colors). Display drivers use this member, for example, in the ChangeDisplaySettings function.
        /// Printer drivers do not use this member.
        /// </summary>
        public uint BitsPerPel;

        /// <summary>
        /// Specifies the width, in pixels, of the visible device surface. Display drivers use this member, for example, in the
        /// ChangeDisplaySettings function. Printer drivers do not use this member.
        /// </summary>
        public uint PixelsWidth;

        /// <summary>
        /// Specifies the height, in pixels, of the visible device surface. Display drivers use this member, for example, in the
        /// ChangeDisplaySettings function. Printer drivers do not use this member.
        /// </summary>
        public uint PixelsHeight;

        /// <summary>DUMMYUNIONNAME2</summary>
        private DevModeU2 Union2;

        [StructLayout(LayoutKind.Explicit)]
        private struct DevModeU2
        {
            [FieldOffset(0)]
            public DisplayEnum DisplayFlags;

            [FieldOffset(0)]
            public NupEnum Nup;
        }

        /// <summary>
        /// <para>Specifies the device's display mode. This member can be a combination of the following values.</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DM_GRAYSCALE</term>
        /// <term>Specifies that the display is a noncolor device. If this flag is not set, color is assumed.</term>
        /// </item>
        /// <item>
        /// <term>DM_INTERLACED</term>
        /// <term>Specifies that the display mode is interlaced. If the flag is not set, noninterlaced is assumed.</term>
        /// </item>
        /// </list>
        /// <para>
        /// Display drivers use this member, for example, in the ChangeDisplaySettings function. Printer drivers do not use this member.
        /// </para>
        /// </summary>
        public DisplayEnum DisplayFlags { get => Union2.DisplayFlags; set => Union2.DisplayFlags = value; }

        /// <summary>
        /// <para>Specifies where the NUP is done. It can be one of the following.</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMNUP_SYSTEM</term>
        /// <term>The print spooler does the NUP.</term>
        /// </item>
        /// <item>
        /// <term>DMNUP_ONEUP</term>
        /// <term>The application does the NUP.</term>
        /// </item>
        /// </list>
        /// </summary>
        public NupEnum Nup { get => Union2.Nup; set => Union2.Nup = value; }

        /// <summary>
        /// <para>
        /// Specifies the frequency, in hertz (cycles per second), of the display device in a particular mode. This value is also known
        /// as the display device's vertical refresh rate. Display drivers use this member. It is used, for example, in the
        /// ChangeDisplaySettings function. Printer drivers do not use this member.
        /// </para>
        /// <para>
        /// When you call the EnumDisplaySettings function, the <c>dmDisplayFrequency</c> member may return with the value 0 or 1. These
        /// values represent the display hardware's default refresh rate. This default rate is typically set by switches on a display
        /// card or computer motherboard, or by a configuration program that does not use display functions such as ChangeDisplaySettings.
        /// </para>
        /// </summary>
        public uint DisplayFrequency;

        /// <summary>
        /// <para>
        /// Specifies how ICM is handled. For a non-ICM application, this member determines if ICM is enabled or disabled. For ICM
        /// applications, the system examines this member to determine how to handle ICM support. This member can be one of the following
        /// predefined values, or a driver-defined value greater than or equal to the value of DMICMMETHOD_USER.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMICMMETHOD_NONE</term>
        /// <term>Specifies that ICM is disabled.</term>
        /// </item>
        /// <item>
        /// <term>DMICMMETHOD_SYSTEM</term>
        /// <term>Specifies that ICM is handled by Windows.</term>
        /// </item>
        /// <item>
        /// <term>DMICMMETHOD_DRIVER</term>
        /// <term>Specifies that ICM is handled by the device driver.</term>
        /// </item>
        /// <item>
        /// <term>DMICMMETHOD_DEVICE</term>
        /// <term>Specifies that ICM is handled by the destination device.</term>
        /// </item>
        /// </list>
        /// <para>
        /// The printer driver must provide a user interface for setting this member. Most printer drivers support only the
        /// DMICMMETHOD_SYSTEM or DMICMMETHOD_NONE value. Drivers for PostScript printers support all values.
        /// </para>
        /// </summary>
        public IcmMethodEnum ICMMethod;

        /// <summary>
        /// <para>
        /// Specifies which color matching method, or intent, should be used by default. This member is primarily for non-ICM
        /// applications. ICM applications can establish intents by using the ICM functions. This member can be one of the following
        /// predefined values, or a driver defined value greater than or equal to the value of DMICM_USER.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMICM_ABS_COLORIMETRIC</term>
        /// <term>
        /// Color matching should optimize to match the exact color requested without white point mapping. This value is most appropriate
        /// for use with proofing.
        /// </term>
        /// </item>
        /// <item>
        /// <term>DMICM_COLORIMETRIC</term>
        /// <term>
        /// Color matching should optimize to match the exact color requested. This value is most appropriate for use with business logos
        /// or other images when an exact color match is desired.
        /// </term>
        /// </item>
        /// <item>
        /// <term>DMICM_CONTRAST</term>
        /// <term>
        /// Color matching should optimize for color contrast. This value is the most appropriate choice for scanned or photographic
        /// images when dithering is desired.
        /// </term>
        /// </item>
        /// <item>
        /// <term>DMICM_SATURATE</term>
        /// <term>
        /// Color matching should optimize for color saturation. This value is the most appropriate choice for business graphs when
        /// dithering is not desired.
        /// </term>
        /// </item>
        /// </list>
        /// </summary>
        public IcmEnum ICMIntent;

        /// <summary>
        /// <para>
        /// Specifies the type of media being printed on. The member can be one of the following predefined values, or a driver-defined
        /// value greater than or equal to the value of DMMEDIA_USER.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMMEDIA_STANDARD</term>
        /// <term>Plain paper.</term>
        /// </item>
        /// <item>
        /// <term>DMMEDIA_GLOSSY</term>
        /// <term>Glossy paper.</term>
        /// </item>
        /// <item>
        /// <term>DMMEDIA_TRANSPARENCY</term>
        /// <term>Transparent film.</term>
        /// </item>
        /// </list>
        /// <para>
        /// To retrieve a list of the available media types for a printer, use the DeviceCapabilities function with the DC_MEDIATYPES flag.
        /// </para>
        /// </summary>
        public MediaEnum MediaType;

        /// <summary>
        /// <para>
        /// Specifies how dithering is to be done. The member can be one of the following predefined values, or a driver-defined value
        /// greater than or equal to the value of DMDITHER_USER.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term>DMDITHER_NONE</term>
        /// <term>No dithering.</term>
        /// </item>
        /// <item>
        /// <term>DMDITHER_COARSE</term>
        /// <term>Dithering with a coarse brush.</term>
        /// </item>
        /// <item>
        /// <term>DMDITHER_FINE</term>
        /// <term>Dithering with a fine brush.</term>
        /// </item>
        /// <item>
        /// <term>DMDITHER_LINEART</term>
        /// <term>
        /// Line art dithering, a special dithering method that produces well defined borders between black, white, and gray scaling. It
        /// is not suitable for images that include continuous graduations in intensity and hue, such as scanned photographs.
        /// </term>
        /// </item>
        /// <item>
        /// <term>DMDITHER_GRAYSCALE</term>
        /// <term>Device does gray scaling.</term>
        /// </item>
        /// </list>
        /// </summary>
        public DitherEnum DitherType;

        /// <summary>Not used; must be zero.</summary>
        public uint Reserved1;

        /// <summary>Not used; must be zero.</summary>
        public uint Reserved2;

        /// <summary>This member must be zero.</summary>
        public uint PanningWidth;

        /// <summary>This member must be zero.</summary>

        public uint PanningHeight;

        public DevMode()
        {
            Size = (ushort)Marshal.SizeOf(typeof(DevMode));
            //SpecVersion = SpecVersionEnum.WinXP;
        }
        public DevMode(SpecVersionEnum version)
        {
            Size = (ushort)Marshal.SizeOf(typeof(DevMode));
            SpecVersion = version;
        }

    }

    //[LibraryImport("gdi32.dll", StringMarshalling = StringMarshalling.Utf16)]
    //public static partial nint CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, nint lpInitData);
    [DllImport("gdi32.dll")]
    public static extern nint CreateDC(string lpszDriver, string lpszDevice,
        string lpszOutput, nint lpInitData);


    [LibraryImport("gdi32.dll", EntryPoint = "DeleteDC")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DeleteDC(nint hdc);

    public enum DeviceCap
    {
        /// <summary>
        /// Device driver version
        /// </summary>
        DriverVersion = 0,
        /// <summary>
        /// Device classification
        /// </summary>
        Technology = 2,
        /// <summary>
        /// Horizontal size in millimeters
        /// </summary>
        HorzSize = 4,
        /// <summary>
        /// Vertical size in millimeters
        /// </summary>
        VertSize = 6,
        /// <summary>
        /// Horizontal width in pixels
        /// </summary>
        HorzRes = 8,
        /// <summary>
        /// Vertical height in pixels
        /// </summary>
        VertRes = 10,
        /// <summary>
        /// Number of bits per pixel
        /// </summary>
        BitsPixel = 12,
        /// <summary>
        /// Number of planes
        /// </summary>
        Planes = 14,
        /// <summary>
        /// Number of brushes the device has
        /// </summary>
        NumBrushes = 16,
        /// <summary>
        /// Number of pens the device has
        /// </summary>
        NumPens = 18,
        /// <summary>
        /// Number of markers the device has
        /// </summary>
        NumMarkers = 20,
        /// <summary>
        /// Number of fonts the device has
        /// </summary>
        NumFonts = 22,
        /// <summary>
        /// Number of colors the device supports
        /// </summary>
        NumColors = 24,
        /// <summary>
        /// Size required for device descriptor
        /// </summary>
        PDeviceSize = 26,
        /// <summary>
        /// Curve capabilities
        /// </summary>
        CurveCaps = 28,
        /// <summary>
        /// Line capabilities
        /// </summary>
        LineCaps = 30,
        /// <summary>
        /// Polygonal capabilities
        /// </summary>
        PolygonalCaps = 32,
        /// <summary>
        /// Text capabilities
        /// </summary>
        TextCaps = 34,
        /// <summary>
        /// Clipping capabilities
        /// </summary>
        ClipCaps = 36,
        /// <summary>
        /// Bitblt capabilities
        /// </summary>
        RasterCaps = 38,
        /// <summary>
        /// Length of the X leg
        /// </summary>
        AspectX = 40,
        /// <summary>
        /// Length of the Y leg
        /// </summary>
        AspectY = 42,
        /// <summary>
        /// Length of the hypotenuse
        /// </summary>
        AspectXY = 44,
        /// <summary>
        /// Shading and Blending caps
        /// </summary>
        ShadeBlendCaps = 45,

        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        LogPixelsX = 88,
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        LogPixelsY = 90,

        /// <summary>
        /// Number of entries in physical palette
        /// </summary>
        SizePalette = 104,
        /// <summary>
        /// Number of reserved entries in palette
        /// </summary>
        NumReserved = 106,
        /// <summary>
        /// Actual color resolution
        /// </summary>
        ColorRes = 108,

        // Printing related DeviceCaps. These replace the appropriate Escapes
        /// <summary>
        /// Physical Width in device units
        /// </summary>
        PhysicalWidth = 110,
        /// <summary>
        /// Physical Height in device units
        /// </summary>
        PhysicalHeight = 111,
        /// <summary>
        /// Physical Printable Area x margin
        /// </summary>
        PhysicalOffsetX = 112,
        /// <summary>
        /// Physical Printable Area y margin
        /// </summary>
        PhysicalOffsetY = 113,
        /// <summary>
        /// Scaling factor x
        /// </summary>
        ScalingFactorX = 114,
        /// <summary>
        /// Scaling factor y
        /// </summary>
        ScalingFactorY = 115,

        /// <summary>
        /// Current vertical refresh rate of the display device (for displays only) in Hz
        /// </summary>
        VRefresh = 116,
        /// <summary>
        /// Vertical height of entire desktop in pixels
        /// </summary>
        DesktopVertRes = 117,
        /// <summary>
        /// Horizontal width of entire desktop in pixels
        /// </summary>
        DesktopHorzRes = 118,
        /// <summary>
        /// Preferred blt alignment
        /// </summary>
        BltAlignment = 119
    }





    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern int GetDeviceCaps(nint hdc, DeviceCap capindex);


    public enum CombineRgnStyles:int
    {
        And         =1,
        Or          =2,
        XOr         =3,
        Diff        =4,
        Copy        =5,
        Min         =And,
        Max         =Copy
    }

    [DllImport("gdi32.dll")]
    public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1,
        IntPtr hrgnSrc2, CombineRgnStyles fnCombineMode);
}