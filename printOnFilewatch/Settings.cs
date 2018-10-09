using System;
using System.Drawing;

namespace DirPrintWatcher
{
    class WatcherSettings
    {
        public string WatcherDirectory { get; }        
        public string PrinterName { get; }        
        public string FileFilter { get; set; }
        public string Logo { get; }
        public string ValidatorString { get; }
        public string SerialPort { get; }
        public int Keep { get; }
        public Font WinFont { get; }
        public int RemoveTopLines { get; }

        public WatcherSettings()
        {
            WatcherDirectory = Properties.Settings.Default.WatcherDirectory;            
            PrinterName = Properties.Settings.Default.WinPrinterName;            
            FileFilter = Properties.Settings.Default.FileFilter;
            Logo = Properties.Settings.Default.HeaderLogo;
            SerialPort = Properties.Settings.Default.SerialPort;
            Keep = Properties.Settings.Default.Keep;
            WinFont = Properties.Settings.Default.WinFont;
            RemoveTopLines = Properties.Settings.Default.WinRemoveLines;

            ValidatorString = "";
            string[] stringArray = Properties.Settings.Default.ValidatorHexBytes.Split(' ');
            foreach(string hexstring in stringArray)
            {
                if(hexstring.Length == 2)
                {
                    char hex = Convert.ToChar(Convert.ToInt32(hexstring, 16));
                    ValidatorString = String.Concat(ValidatorString, hex);
                }                
            }
        }

        private void Refresh()
        {
            //Add settings refresh in the future?
        }
    }
}
