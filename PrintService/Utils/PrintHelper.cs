using System;
using System.Drawing.Printing;
using EvoPdf.PdfPrint;

namespace PrintService.Utils
{
    public class PrintHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("PrintHelper");

        public static bool PrintXls(string path, bool vertical)
        {
            log.Info("本地文档：" + path);
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            try
            {
                // 打开xls文件
                path = System.Web.HttpContext.Current.Server.MapPath(path);
                log.Info("本地文档：" + path);
                workbook = excel.Workbooks.Open(path);
                workbook.EnvelopeVisible = false;
                // Sheet1
                worksheet = workbook.Sheets[1];
                log.Info(worksheet.Name);
                if (vertical)
                {
                    worksheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlPortrait;
                } else
                {
                    worksheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
                }
                // 直接打印
                worksheet.PrintOutEx();
                // 保存
                workbook.Save();
                //
                log.Info("直接打印完成");
            } catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
            finally
            {
                // 关闭Excel
                if (workbook != null) {
                    workbook.Close();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }
                if (excel != null)
                {
                    excel.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                    excel = null;
                }
            }
            return true;
        }

        public static bool PrintWord(string path, bool vertical)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = null;
            try
            {
                // 取得word文档
                object templateFile = System.Web.HttpContext.Current.Server.MapPath(path);
                log.Info("本地文档：" + templateFile);
                // 加载
                doc = word.Documents.Add(ref templateFile);
                if (vertical)
                {
                    doc.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientPortrait;
                } else
                {
                    doc.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientLandscape;
                }
                // 直接打印
                doc.PrintOut();
                //
                log.Info("直接打印完成");
            } catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
            finally
            {
                // 不保存文档
                object saveChange = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                // 退出
                if (doc!= null)
                {
                    doc.Close(ref saveChange);
                }
                if (word != null)
                {
                    word.Quit();
                }
            }
            return true;
        }

        public static bool PrintPdf(string path, bool vertical)
        {
            string fileName = "PDF Document";
            if (path == "")
            {
                return false;
            }
            try
            {
                //
                path = System.Web.HttpContext.Current.Server.MapPath(path);
                //
                PrintDocument pd = new PrintDocument();

                string defaultPrinter = pd.PrinterSettings.PrinterName;

                if (defaultPrinter.Contains("未设置默认打印机"))
                {
                    if (PrinterSettings.InstalledPrinters.Count > 0)
                    {
                        defaultPrinter = PrinterSettings.InstalledPrinters[0];
                    }
                }
                //
                
                log.Info("打印机：" + defaultPrinter);
                log.Info("文件路径：" + path);
                //
                PdfPrint pdfPrint = new PdfPrint();
                // set the license key
                pdfPrint.LicenseKey = "oy08LDo/LDwsOiI8LD89Ij0+IjU1NTUsPA==";
                // set file name
                pdfPrint.DocumentName = fileName;
                // set printer
                pdfPrint.PrinterSettings.PrinterName = defaultPrinter;
                // 
                pdfPrint.DefaultPageSettings.Landscape = !vertical;
                //
                pdfPrint.Print(path);
            } catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }

            return true;
        }
    }
}