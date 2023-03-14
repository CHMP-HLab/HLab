using System;
using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public class Winternl
{
    public enum ProcessInfoClass
    {
        ProcessBasicInformation = 0x00,
        ProcessDebugPort = 0x07,
        ProcessExceptionPort = 0x08,
        ProcessAccessToken = 0x09,
        ProcessWow64Information = 0x1A,
        ProcessImageFileName = 0x1B,
        ProcessDebugObjectHandle = 0x1E,
        ProcessDebugFlags = 0x1F,
        ProcessExecuteFlags = 0x22,
        ProcessInstrumentationCallback = 0x28,
        MaxProcessInfoClass = 0x64,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessBasicInformation
    {
        public nint Reserved1;
        public nint PebBaseAddress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public nint[] Reserved2;
        public nint UniqueProcessId;
        public nint Reserved3;
    }

    [DllImport("ntdll.dll")]
    public static extern uint NtQueryInformationProcess(
        nint processHandle, 
        ProcessInfoClass processInformationClass, 
        nint processInformation, 
        uint processInformationLength, 
        out uint returnLength);


}


public static partial class ProcessThreadsApi
{
    [LibraryImport("kernel32.dll")]
    public static partial uint GetCurrentThreadId();


    [Flags]
    public enum OpenProcessDesiredAccessFlags : uint
    {
        VmRead = 0x0010,
        QueryInformation = 0x0400,
    }

    [DllImport("kernel32.dll")]
    public static extern nint OpenProcess(OpenProcessDesiredAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [StructLayout(LayoutKind.Sequential)]
    public struct UnicodeString
    {
        public ushort Length;
        public ushort MaximumLength;
        public nint Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlUserProcessParameters
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public nint[] Reserved2;
        public UnicodeString ImagePathName;
        public UnicodeString CommandLine;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PEB {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reserved1;
        public byte BeingDebugged;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public byte[] Reserved2;
        public nint LoaderData; //PPEB_LDR_DATA
        public nint ProcessParameters; //PRTL_USER_PROCESS_PARAMETERS
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 520)]
        public nint PostProcessInitRoutine; //PPS_POST_PROCESS_INIT_ROUTINE
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 136)]
        public ulong SessionId;
    }
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, nint lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(nint hObject);

}