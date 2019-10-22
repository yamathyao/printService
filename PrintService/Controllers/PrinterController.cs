using PrintService.Models;
using PrintService.Utils;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Web.Http;
namespace PrintService.Controllers
{
    public class PrinterController : ApiController
    {

        private log4net.ILog log = log4net.LogManager.GetLogger("PrinterController");

        [HttpPost]
        // api/printer
        public string PrintDoc([FromBody] PrintAttr attr)
        {
            string docUrl = attr.DocUrl;

            bool vertical = attr.Vertical;

            string docPath = "";

            log.Info("文档下载：" + docUrl);

            if (string.IsNullOrWhiteSpace(docUrl))
            {
                return "文档URL为空";
            }
            try
            {
                docPath = DownloadDoc(docUrl);
                string suffix = getSuffix(docUrl);
                // 文档下载
                if (!string.IsNullOrWhiteSpace(docPath))
                {
                    bool success = false;
                    // 打印文档
                    if ("pdf".Equals(suffix))
                    {
                        //success = PrintHelper.PrintPdf(docPath, vertical);
                        FirPrint(docPath);
                        success = true;
                    }
                    if ("xls".Equals(suffix) || "xlsx".Equals(suffix))
                    {
                        success = PrintHelper.PrintXls(docPath, vertical);
                    }
                    if ("doc".Equals(suffix) || "docx".Equals(suffix))
                    {
                        success = PrintHelper.PrintWord(docPath, vertical);
                    }
                    if ("jpg".Equals(suffix.ToLowerInvariant()) || "png".Equals(suffix.ToLowerInvariant()))
                    {
                        DrawPrintHelper helper = new DrawPrintHelper();
                        success = helper.StartPrint(docPath, "image");
                    }
                    if (success)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Fail";
                    }
                }
                else
                {
                    return "文档下载失败！";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return "Fail";
            }
            finally
            {
                //
                string file = System.Web.HttpContext.Current.Server.MapPath(docPath);
                log.Info("要删除的文件：" + file);
                if (File.Exists(file))
                {
                    File.Delete(file);
                    log.Info("文件删除完成");
                }
            }
        }

        // 下载
        private string DownloadDoc(string url)
        {
            string fileName = getFileName(url);
            //
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            //请求网络路径地址
            request = (HttpWebRequest)WebRequest.Create(url);
            // 超时时间
            request.Timeout = 5000;
            //获得请求结果
            response = (HttpWebResponse)request.GetResponse();
            string virtualPath = ("\\download\\doc");
            string path = System.Web.HttpContext.Current.Server.MapPath("\\download\\doc");
            // 创建文件路径
            if (!Directory.Exists(path))
            {
                if (path != null)
                {
                    Directory.CreateDirectory(path);
                }
            }
            //
            virtualPath = virtualPath + "\\" + fileName;
            path = path + "\\" + fileName;
            //
            Stream stream = response.GetResponseStream();
            Stream sos = new FileStream(path, FileMode.Create);
            byte[] doc = new byte[1024];
            int total = stream.Read(doc, 0, doc.Length);
            while (total > 0)
            {
                //之后再输出内容
                sos.Write(doc, 0, total);
                total = stream.Read(doc, 0, doc.Length);
            }
            stream.Close();
            stream.Dispose();
            sos.Close();
            sos.Dispose();
            //
            return virtualPath;
        }

        // 取得文件名
        private string getFileName(string path)
        {
            string fileName = "";
            if (path == "")
            {
                return "";
            }
            //
            if (path.IndexOf("/") > -1)
            {
                fileName = path.Substring(path.LastIndexOf("/") + 1);
            }
            else
            {
                fileName = path;
            }
            return fileName;
        }

        // 取得后缀
        private string getSuffix(string path)
        {
            string suffix = "";
            if (string.IsNullOrWhiteSpace(path))
            {
                return "";
            }
            //
            suffix = path.Substring(path.LastIndexOf(".") + 1);
            return suffix;
        }

        //
        private void FirPrint(string path)
        {
            path = System.Web.HttpContext.Current.Server.MapPath(path);
            log.Info("本地路径：" + path);

            PrintDocument pd = new PrintDocument();

            string defaultPrinter = pd.PrinterSettings.PrinterName;

            if (defaultPrinter.Contains("未设置默认打印机"))
            {
                if (PrinterSettings.InstalledPrinters.Count > 0)
                {
                    defaultPrinter = PrinterSettings.InstalledPrinters[0];
                }
            }

            log.Info("默认打印机：" + defaultPrinter);

            Process p = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.CreateNoWindow = true;

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            //startInfo.UseShellExecute = true;
            startInfo.UseShellExecute = false;

            //startInfo.FileName = path;
            startInfo.FileName = @"C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe";

            startInfo.Verb = "print";

            //startInfo.Arguments = @"/p /h \" + path + "\"\"" + defaultPrinter + "\"";
            startInfo.Arguments = string.Format(@"/p /h {0}", path);

            p.StartInfo = startInfo;

            log.Info(startInfo.Arguments);
            log.Info(startInfo.Verb);

            p.Start();

            p.WaitForExit(10000);

            //p.Dispose();


            int counter = 0;
            while (!p.HasExited)
            {
                System.Threading.Thread.Sleep(1000);
                counter += 1;
                if (counter == 5) break;
            }
            if (!p.HasExited)
            {
                log.Info("Kill打印");
                p.CloseMainWindow();
                p.Kill();
            }
        }
    }
}
