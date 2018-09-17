using System.Collections.Generic;
using System.IO;

namespace printOnFilewatch
{
    class WatcherPrintStack
    {
        private HashSet<string> fileSeen;
        private System.Timers.Timer debounceTimer;        

        public WatcherPrintStack()
        {
            fileSeen = new HashSet<string>();
            debounceTimer = new System.Timers.Timer()
            {
                Interval = 1000,
                AutoReset = false
            };
            debounceTimer.Elapsed += (s, e) => fileSeen.Clear();
        }

        public void AddFile(string file)
        {
            if (fileSeen.Add(file))
            {
                PrintFile(file);
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
                
        protected void PrintFile(string file)
        {
            debounceTimer.Stop();
            while (File.Exists(file) && FileIsLocked(file))
            {
                System.Threading.Thread.Sleep(100);
            }

            if (File.Exists(file))
            {
                var wp = new WatcherPrinter(file);
                wp.Print();
            }
            debounceTimer.Start();
        }
    }
}
