using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

//4. Refactoring using DIP  - Inverting Dependencies
namespace DIP.Change01
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
        private FileLogger _fileLogger;
        private TextScanner _textScanner;
        private ConsoleTerminal _consoleTerminal;
        private WebText _webText;

        public WebAnalytic()
            : this(new ConsoleTerminal(), new WebText("http://www.google.com"), new FileLogger(), new TextScanner())
        {
        }

        public WebAnalytic(ConsoleTerminal consoleTerminal, WebText webText, FileLogger fileLogger, TextScanner textScanner)
        {
            _consoleTerminal = consoleTerminal;
            _webText = webText;
            _fileLogger = fileLogger;            
            _textScanner = textScanner;
        }

        public void Run()
        {
            _fileLogger.WriteLine(string.Format("Web text from {0}", _webText.WebAddress));
            string webText = _webText.GetText();
            var reportText = _textScanner.Scan(webText);

            _consoleTerminal.Write(string.Format("Result of scanning web text from {0}\n{1}", _webText, reportText));
            _consoleTerminal.WaitKey();
        }
    }
    
    internal class WebText
    {
        public string WebAddress { get; private set; }
        public WebText(string webAddress)
        {
            WebAddress = webAddress;
        }

        public string GetText()
        {
            using (var client = new HttpClient())
            {
                return client.GetStringAsync(WebAddress).Result;
            }
        }
    }

    internal class ConsoleTerminal
    {
        void Write(string text)
        {
            Console.Write(text);
        }

        void WaitKey()
        {
            Console.ReadKey();
        }
    }

    internal class FileWriterStream
    {
        private string _filePath;
        public FileWriterStream(string filePath)
        {
            _filePath = filePath;
        }

        StreamWriter OpenStream()
        {
            return File.AppendText(_filePath);
        }
    }

    internal class FileLogger
    {
        private FileWriterStream _writerStream;

        public FileLogger()
            : this(new FileWriterStream(@"D:\log.txt"))
        {
        }

        public FileLogger(IWriterStream writerStream)
        {
            _writerStream = writerStream;
        }

        public void WriteLine(string text)
        {
            using (var stream = _writerStream.OpenStream())
            {
                stream.Write(text + Environment.NewLine);
            }
        }
    }

    internal class TextScanner
    {
        private FileLogger _fileLogger;

        public TextScanner()
            : this(new FileLogger())
        {
        }

        public TextScanner(FileLogger fileLogger)
        {
            _fileLogger = fileLogger;
        }

        public string Scan(string text)
        {
            var reportText = string.Format("Number of characters scanned: {0}", text.Length);
            //can be extended with more analysis
            _fileLogger.WriteLine(reportText);
            return reportText;
        }
    }
}
