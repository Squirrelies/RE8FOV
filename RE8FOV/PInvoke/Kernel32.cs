using System;
using System.Runtime.InteropServices;

namespace RE8FOV.PInvoke
{
    public static class Kernel32
    {
        [DllImport(nameof(Kernel32), SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessRightsFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, void* lpBaseAddress, void* lpBuffer, UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, void* lpBaseAddress, void* lpBuffer, UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);

        // ACCESS_MASK documentation: https://docs.microsoft.com/en-us/windows/win32/secauthz/access-mask-format
        [Flags]
        public enum GenericAccessRightsFlags : uint
        {
            ALL = 1U << 28, // 0x10000000,
            EXECUTE = 1U << 29, // 0x20000000,
            WRITE = 1U << 30, // 0x40000000,
            READ = 1U << 31, // 0x80000000,
        }

        [Flags]
        public enum StandardAccessRightsFlags : uint
        {
            DELETE = 1U << 16, // 0x00010000,
            READ_CONTROL = 1U << 17, // 0x00020000,
            WRITE_DAC = 1U << 18, // 0x00040000,
            WRITE_OWNER = 1U << 19, // 0x00080000,

            SYNCHRONIZE = 1U << 20, // 0x00100000,
            // = 1U << 21, // 0x00200000,
            // = 1U << 22, // 0x00400000,
            // = 1U << 23, // 0x00800000,

            ACCESS_SYSTEM_SECURITY = 1U << 24, // 0x01000000,
            MAXIMUM_ALLOWED = 1U << 25, // 0x02000000,
            // = 1U << 26, // 0x04000000,
            // = 1U << 27, // 0x08000000,

            STANDARD_RIGHTS_ALL = DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER | SYNCHRONIZE, // 0x001F0000,
            STANDARD_RIGHTS_EXECUTE = READ_CONTROL, // 0x00020000,
            STANDARD_RIGHTS_READ = READ_CONTROL, // 0x00020000,
            STANDARD_RIGHTS_REQUIRED = DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER, // 0x000F0000,
            STANDARD_RIGHTS_WRITE = READ_CONTROL, // 0x00020000,
        }

        [Flags]
        public enum ProcessAccessRightsFlags : uint
        {
            TERMINATE = 1U << 0, // 0x00000001,
            CREATE_THREAD = 1U << 1, // 0x00000002,
            // = 1U << 2, // 0x00000004,
            VM_OPERATION = 1U << 3, // 0x00000008,

            VM_READ = 1U << 4, // 0x00000010,
            VM_WRITE = 1U << 5, // 0x00000020,
            DUP_HANDLE = 1U << 6, // 0x00000040,
            CREATE_PROCESS = 1U << 7, // 0x00000080,

            SET_QUOTA = 1U << 8, // 0x00000100,
            SET_INFORMATION = 1U << 9, // 0x00000200,
            QUERY_INFORMATION = 1U << 10, // 0x00000400,
            SUSPEND_RESUME = 1U << 11, // 0x00000800,

            QUERY_LIMITED_INFORMATION = 1U << 12, // 0x00001000,
            // = 1U << 13, // 0x00002000,
            // = 1U << 14, // 0x00004000,
            // = 1U << 15, // 0x00008000,
        }
    }
}
