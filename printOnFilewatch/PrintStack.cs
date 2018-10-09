using System.Collections.Generic;
using System.IO;
using System;
using System.Drawing;
using System.Linq;


namespace DirPrintWatcher
{
    class WatcherPrintStack
    {
        private HashSet<string> fileSeen;
        private System.Timers.Timer debounceTimer;
        private bool MultipleStarted = false;        
        private MemoryStream ms = null;
        private Actions actions;

        public WatcherPrintStack()
        {
            fileSeen = new HashSet<string>();
            debounceTimer = new System.Timers.Timer()
            {
                Interval = 1000,
                AutoReset = false
            };
            debounceTimer.Elapsed += (s, e) => fileSeen.Clear();

            actions = new Actions();
        }

        public void AddFile(string file)
        {
            if (fileSeen.Add(file))
            {
                ProcessFile(file);
            }
        }


        protected bool FileIsLocked(string path)
        {
            FileStream fs = null;
            FileInfo fi = new FileInfo(path);

            try
            {
                fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return false;
        }
                
        protected void ProcessFile(string file)
        {
            debounceTimer.Stop();
            while (File.Exists(file) && FileIsLocked(file))
            {
                System.Threading.Thread.Sleep(100);
            }

            if (File.Exists(file))
            {
                
                //GET ACTION
                var action = actions.MatchAction(file);
                Console.WriteLine("Processing {0} \n Action taken: '{1}'",file,action.ToString());

                //PROCESS ACTION
                if(!MultipleStarted)
                {
                    switch (action)
                    {
                        case PossibleActions.Print:
                            Print(file);
                            break;
                        case PossibleActions.PrintAndCut:
                            Print(file);
                            Cut();
                            break;
                        case PossibleActions.Cut:
                            Cut();
                            break;
                        case PossibleActions.Default:
                            WinPrint(file);
                            break;
                        case PossibleActions.MultiTranStart:
                            MultipleStarted = true;
                            AddToMemStream(file);
                            break;
                        case PossibleActions.ValidatorPrint:
                            Print(file);
                            break;
                        case PossibleActions.Ignore:
                            break;                        
                        default:
                            break;
                    }
                }
                else if(action == PossibleActions.ValidatorPrint)
                {
                    Print(file);
                }
                else if(action == PossibleActions.MultiTranEnd)
                {
                    MultipleStarted = false;
                    AddToMemStream(file);
                    PrintAndCloseMemStream();
                }
                else
                {
                    AddToMemStream(file);
                }

                CleanupFiles();
            }
            debounceTimer.Start();
        }


        private void Print(string file)
        {            
            StreamReader sr = null;
            SerialPrinter printer = new SerialPrinter("\\\\.\\" + DirPrintWatcher.Settings.SerialPort);

            try
            {
                
                sr = new StreamReader(file);
                printer.Print(sr);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                printer.Cleanup();
            }
        }

        private void Cut()
        {
            Cut(SerialPrinter.CutType.Full);      
        }

        private void Cut(SerialPrinter.CutType CutType)
        {
            byte[] c_bytes = { 0x1B, 0x63, 0x30, 0x02, //GS c 0 0x02 --> Select roll paper
                0x1D, 0x56, (byte)CutType, 0x10 }; //ESC V 0x00 --> CUT Command
            string test = System.Text.Encoding.UTF8.GetString(c_bytes);
            SerialPrinter printer = new SerialPrinter("\\\\.\\" + DirPrintWatcher.Settings.SerialPort);
            printer.Print(c_bytes);
            printer.Cleanup();
        }

        private void AddToMemStream(string file)
        {
            var fs = new FileStream(file, FileMode.Open, FileAccess.Read);

            //Discard ESC POS control characters
            long posTemp = fs.Position;
            int peek = fs.ReadByte();
            fs.Position = posTemp;
            while (peek == 64 || peek < 32 || peek > 122)
            {
                fs.ReadByte();
                posTemp = fs.Position;
                peek = fs.ReadByte();
                fs.Position = posTemp;
            }

            if (ms == null)
                ms = new MemoryStream();

            long offset = ms.Length;
            ms.SetLength(offset + fs.Length - fs.Position);
            fs.Read(ms.GetBuffer(), (int)offset, (int)fs.Length);
            fs.Close();
        }      

        private void PrintAndCloseMemStream()
        {
            StreamReader sr = new StreamReader(ms);            
            var printer = new WinPrinter(DirPrintWatcher.Settings.PrinterName, DirPrintWatcher.Settings.WinFont, DirPrintWatcher.Settings.Logo);
            //remove top x lines
            for (int i = 0; i < DirPrintWatcher.Settings.RemoveTopLines && sr.ReadLine() != null; i++) { };
            printer.Print(sr);
            sr.Close();
            if (ms != null)
                ms.Close();
            ms = null;
            printer.Cleanup();
        }
                
        private void WinPrint(string file)
        {
            var printer = new WinPrinter(DirPrintWatcher.Settings.PrinterName, DirPrintWatcher.Settings.WinFont, DirPrintWatcher.Settings.Logo);
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(file);
                //remove top x lines 
                for (int i = 0; i < DirPrintWatcher.Settings.RemoveTopLines && sr.ReadLine() != null; i++) { };
                printer.Print(sr);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                printer.Cleanup();
            }
        }

        
        private void CleanupFiles()
        {
            var filesByDate = new DirectoryInfo(DirPrintWatcher.Settings.WatcherDirectory)
                .GetFiles()
                .OrderByDescending(f => f.CreationTime)
                .ToList();

            for (int i = DirPrintWatcher.Settings.Keep; i < filesByDate.Count; i++)
            {
                if (filesByDate[i].Exists)
                    filesByDate[i].Delete();
            }
        }
    }
}
