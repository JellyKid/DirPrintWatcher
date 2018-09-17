using System;
using System.IO;

namespace printOnFilewatch
{
    class DirPrintWatcher
    {
        private WatcherPrintStack ps;
        private FileSystemWatcher fsw;
        private static WatcherSettings settings;

        internal static WatcherSettings Settings { get => settings;}

        public DirPrintWatcher()
        {         
            ps = new WatcherPrintStack();
            settings = new WatcherSettings();
        }

        public void Start()
        {
            fsw = new FileSystemWatcher()
            {
                Path = Settings.WatcherDirectory,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                Filter = Settings.FileFilter
            };

            fsw.Created += (s, e) => ps.AddFile(e.FullPath);
            fsw.Changed += (s, e) => ps.AddFile(e.FullPath);

            fsw.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            fsw.Dispose();      
        }

    }    
}
