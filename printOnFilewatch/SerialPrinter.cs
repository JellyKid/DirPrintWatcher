using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace DirPrintWatcher
{
    class SerialPrinter : Printer
    {        
        public enum CutType {
            Full = 0x41,
            Partial = 0x42
        };
        private IntPtr hcom = IntPtr.Zero;
        private bool PortOpen = false;
        private enum CommErrors : UInt32
        {
            ERROR_INVALID_NAME = 123,
            ERROR_ACCESS_DENIED = 5,
            ERROR_IO_PENDING = 997
        }

        public SerialPrinter(string CommPort)
        {
            hcom = Kernel32.CreateFile(
                CommPort, //port name
                Kernel32.GENERIC_WRITE, //write only
                0, //no sharing
                IntPtr.Zero, //no security
                Kernel32.OPEN_EXISTING, //open existing port only
                Kernel32.FILE_FLAG_OVERLAPPED, //async(overlapped) I/O
                IntPtr.Zero); //Null for Comm Devices
            if(hcom == (IntPtr)Kernel32.INVALID_HANDLE_VALUE)
            {
                CommErrors e = (CommErrors)Marshal.GetLastWin32Error();
                Console.WriteLine("Error opening serial port: {0}", e.ToString());
            }
            else
            {
                PortOpen = true;
            }
        }

        ~SerialPrinter()
        {
            Cleanup();
        }

        public void Print(byte[] buffer)
        {
            uint numBytes = (uint)buffer.Length;
            uint bytesWritten;
            bool success = Kernel32.WriteFile(
                hcom,//Serialport com handle
                buffer,//buffer to write
                numBytes,//size of buffer
                out bytesWritten,//how many bytes were actually written.... might have to keep trying?
                IntPtr.Zero);//Syncronise
            if (success)
            {
                Console.WriteLine("Printed {0} of {1} bytes", bytesWritten, numBytes);
            }
            else
            {
                Console.WriteLine("Failed to print");
            }
        }

        public void Print(string s)
        {
            Print(new ASCIIEncoding().GetBytes(s));
        }
        
        public void Print(StreamReader sr)
        {
            if (!sr.EndOfStream)
            {
                byte[] buffer = new byte[sr.BaseStream.Length];
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                sr.BaseStream.Read(buffer, 0, (int)sr.BaseStream.Length);
                Print(buffer);
            }
        }

        public void Cleanup()
        {
            if (hcom != IntPtr.Zero)
                Kernel32.CloseHandle(hcom);
        }
    }
}
