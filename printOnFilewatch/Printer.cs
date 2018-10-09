using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;

namespace DirPrintWatcher
{
    public interface Printer
    {
        void Print(StreamReader sr);
        void Cleanup();
    }
}
