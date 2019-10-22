using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace PrintService.Utils
{
    public class DrawPrintHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("DrawPrintHelper");

        private PrintDocument printDoc = new PrintDocument();

        private string streamType;

        private string streamTxt;

        private Image streamImg;

        public DrawPrintHelper()
        {
            printDoc.PrintPage += new PrintPageEventHandler(DocToPrint_PrintPage);
        }

        public bool StartPrint(string path, string streamType)
        {
            bool success = false;
            path = System.Web.HttpContext.Current.Server.MapPath(path);
            try
            {
                if ("image".Equals(streamType))
                {
                    Image image = Image.FromFile(path);
                    this.streamImg = image;
                    this.streamType = streamType;
                    printDoc.Print();
                    success = true;
                    image.Dispose();
                }
                else if ("txt".Equals(streamType))
                {
                    string data = null;
                    string line = null;
                    StreamReader sr = new StreamReader(path);
                    while ((line = sr.ReadLine()) != null)
                    {
                        data += line;
                    }
                    this.streamTxt = data;
                    this.streamType = streamType;
                    success = true;
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                printDoc.Dispose();
            }
            return success;
        }

        private void DocToPrint_PrintPage(Object sender, PrintPageEventArgs e)
        {
            switch (streamType)
            {
                case "txt":
                    string text = null;
                    Font printFont = new Font("Arial", 35, FontStyle.Regular);
                    text = streamTxt;
                    e.Graphics.DrawString(text, printFont, Brushes.Black, e.MarginBounds.X, e.MarginBounds.Y);
                    break;
                case "image":
                    Image image = streamImg;

                    e.Graphics.DrawImage(image, e.Graphics.VisibleClipBounds);

                    break;
                default:
                    break;
            }
        }
    }
}