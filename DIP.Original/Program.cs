using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DIP.Original
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var webAnalytic = new WebAnalytic();
            webAnalytic.Run();
        }
    }

    internal class WebAnalytic
    {        
        public void Run()
        {
            string webAddress = "http://www.google.com";
            FileLogger.Instance.WriteLine(string.Format("Web text from {0}", webAddress));

            using (var client = new HttpClient())
            {
                string webText = client.GetStringAsync(webAddress).Result;

                var textScanner = new TextScanner();
                var reportText = textScanner.Scan(webText);

                Console.Write(string.Format("Result of scanning web text from {0}\n{1}", webAddress, reportText));
            }
            Console.ReadKey();
        }
    }

    internal class FileLogger
    {
        private string _filePath = @"D:\log.txt";

        private static FileLogger _instance;
        public static FileLogger Instance
        {
            get
            {
                return _instance ?? (_instance = new FileLogger());
            }
        }

        private FileLogger()
        {
        }

        public void WriteLine(string text)
        {
            using (var stream = File.AppendText(_filePath))
            {
                stream.Write(text + Environment.NewLine);
            }
        }
    }

    internal class TextScanner
    {
        public string Scan(string text)
        {
            var reportText = string.Format("Number of characters scanned: {0}", text.Length);
            //can be extended with more analysis
            FileLogger.Instance.WriteLine(reportText);
            return reportText;
        }
    }
}
