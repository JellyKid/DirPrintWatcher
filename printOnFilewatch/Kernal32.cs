using System;
using System.Runtime.InteropServices;

namespace DirPrintWatcher
{
    internal class Kernel32
    {    
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode,
                                                 IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes,
                                                 IntPtr hTemplateFile);

        //errors:
        internal const UInt32 ERROR_FILE_NOT_FOUND = 2;
        internal const UInt32 ERROR_INVALID_NAME = 123;
        internal const UInt32 ERROR_ACCESS_DENIED = 5;
        internal const UInt32 ERROR_IO_PENDING = 997;

        //return value:
        internal const Int32 INVALID_HANDLE_VALUE = -1;

        //dwFlagsAndAttributes:
        internal const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

        //dwCreationDisposition:
        internal const UInt32 OPEN_EXISTING = 3;

        //dwDesiredAccess:
        internal const UInt32 GENERIC_READ = 0x80000000;
        internal const UInt32 GENERIC_WRITE = 0x40000000;

        [DllImport("kernel32.dll")]
        internal static extern Boolean CloseHandle(IntPtr hObject);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite,
                                                 out UInt32 lpNumberOfBytesWritten, IntPtr lpOverlapped);

        
    }
}
