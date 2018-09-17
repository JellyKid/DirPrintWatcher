using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;

namespace printOnFilewatch
{
    class WatcherPrinter
    {
        private Font pfont;
        private StreamReader sr;
        private PrintDocument pd;
        private string fp;
        private bool ReadyToPrint;
        private Image image;

        public WatcherPrinter(string file)
        {
            try
            {
                fp = file;
                sr = new StreamReader(fp);
                pfont = new Font(DirPrintWatcher.Settings.Font, DirPrintWatcher.Settings.FontSize, DirPrintWatcher.Settings.FontStyle);
                pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = DirPrintWatcher.Settings.PrinterName;                
                pd.BeginPrint += Pd_BeginPrint;
                pd.PrintPage += (s, e) => PPHandler(e);
                pd.EndPrint += (s, e) => EPHandler(e);
                ReadyToPrint = true;
            }
            catch
            {
                if (sr != null)
                    sr.Close();
                ReadyToPrint = false;
            }
        }

        private void Pd_BeginPrint(object sender, PrintEventArgs e)
        {
            if (DirPrintWatcher.Settings.Logo != null)
            {
                image = Image.FromFile(DirPrintWatcher.Settings.Logo);
            }               
        }

        private void PPHandler(PrintPageEventArgs e)
        {
            float linesPP, ypos, leftMargin, topMargin, lineHeight;
            int count = 0;
            int topLinesDrop = 2;
            string line = null;

            lineHeight = pfont.GetHeight(e.Graphics);
            leftMargin = 0;
            topMargin = lineHeight;
            linesPP = e.MarginBounds.Height / lineHeight;

            if (image != null)
            {                
                GraphicsUnit units = GraphicsUnit.Pixel;
                RectangleF ib = image.GetBounds(ref units);                
                RectangleF LogoBounds = e.PageSettings.PrintableArea;
                LogoBounds.Height = (LogoBounds.Width / ib.Width) * ib.Height;
                LogoBounds.X = 0;
                topMargin += LogoBounds.Height;
                e.Graphics.DrawImage(image, LogoBounds);                
                image = null;                
            }

            while (topLinesDrop > 0 && sr.ReadLine() != null)
                topLinesDrop--;

            while (count < linesPP &&
                ((line = sr.ReadLine()) != null))
            {
                ypos = topMargin + (count * lineHeight);
                e.Graphics.DrawString(line, pfont, Brushes.Black, leftMargin, ypos, new StringFormat());
                count++;
            }

            if (line != null)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;

        }

        private void EPHandler(PrintEventArgs e)
        {
            if (sr != null)
                sr.Close();
            ReadyToPrint = false;

            if (File.Exists(fp))
                File.Delete(fp);
        }

        public void Print()
        {
            if (ReadyToPrint)
            {
                Console.WriteLine("Printing: " + fp);
                pd.Print();
            }
        }
    }
}
