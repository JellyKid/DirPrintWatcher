using System;
using System.Drawing;

namespace printOnFilewatch
{
    class WatcherSettings
    {
        public string WatcherDirectory { get; }
        public string Font { get; }
        public FontStyle FontStyle { get; }
        public string PrinterName { get; }
        public float FontSize { get; }
        public string FileFilter { get; set; }
        public string Logo { get; }

        public WatcherSettings()
        {
            WatcherDirectory = Properties.Settings.Default.WatcherDirectory;
            Font = Properties.Settings.Default.Font;
            PrinterName = Properties.Settings.Default.PrinterName;
            FontSize = Properties.Settings.Default.FontSize;
            FileFilter = Properties.Settings.Default.FileFilter;
            Logo = Properties.Settings.Default.HeaderLogo;
            try
            {
                FontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), Properties.Settings.Default.FontStyle);
            }
            catch
            {
                FontStyle = FontStyle.Regular;
            }
        }

        private void Refresh()
        {
            
        }
    }
}
