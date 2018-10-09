using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;

namespace DirPrintWatcher
{
    class WinPrinter : Printer
    {
        private Font pfont;        
        private PrintDocument pd;        
        private Image image;
        private int topMargin, leftMargin;

        public WinPrinter(string name, Font font) : this(name, font, null, 0, 0)
        {
            
        }

        public WinPrinter(string name, Font font, string logo) : this(name, font, logo, 0, 0)
        {

        }

        public WinPrinter(string name, Font font, string logo, int topmargin, int leftmargin)
        {
            pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = name;
            pfont = font;

            topMargin = topmargin;
            leftMargin = leftmargin;

            if(logo != null)
                image = Image.FromFile(logo);
        }   

        private void PPHandler(PrintPageEventArgs e, StreamReader sr)
        {
            float linesPP, ypos, leftMargin, topMargin, lineHeight;
            int count = 0;            
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

        public void Print(StreamReader sr)
        {
            pd.PrintPage += (s, e) => PPHandler(e, sr);
            pd.Print();
        }

        public void Cleanup()
        {
            if (pd != null)
                pd.Dispose();
        }
        
    }
}
