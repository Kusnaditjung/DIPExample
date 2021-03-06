﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

//4. Refactoring using DIP  - Abstracting Remaining Dependencies
namespace DIP.Change02
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
        private ILogger _logger;
        private IScanner _scanner;
        private IConsole _console;
        private IWebText _webText;

        public WebAnalytic()
            : this(new ConsoleTerminal(), new WebText("http://www.google.com"), new FileLogger(), new TextScanner())
        {
        }

        public WebAnalytic(IConsole console, IWebText webText, ILogger logger, IScanner scanner)
        {
            _console = console;
            _logger = logger;
            _webText = webText;
            _scanner = scanner;
        }

        public void Run()
        {
            _logger.WriteLine(string.Format("Web text from {0}", _webText.WebAddress));
            string webText = _webText.GetText();
            var reportText = _scanner.Scan(webText);

            _console.Write(string.Format("Result of scanning web text from {0}\n{1}", _webText, reportText));
            _console.WaitKey();
        }
    }

    internal interface ILogger
    {
        string WriteLine(string text);
    }

    internal interface IScanner
    {
        string Scan(string text);
    }

    internal interface IWebText
    {
        string WebAddress { get; }
        string GetText();
    }

    internal class WebText : IWebText
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

    internal interface IConsole
    {
        void Write(string text);
        void WaitKey();
    }

    internal class ConsoleTerminal : IConsole
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

    internal interface IWriterStream
    {
        StreamWriter OpenStream();
    }

    internal class FileWriterStream : IWriterStream
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

    internal class FileLogger: ILogger
    {
        private IWriterStream _writerStream;

        public FileLogger(): this(new FileWriterStream(@"D:\log.txt"))
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

    internal class TextScanner: IScanner
    {
        private ILogger _logger;

        public TextScanner()
            : this(new FileLogger())
        {
        }

        public TextScanner(ILogger logger)
        {
            _logger = logger;
        }

        public string Scan(string text)
        {
            var reportText = string.Format("Number of characters scanned: {0}", text.Length);
            //can be extended with more analysis
            _logger.WriteLine(reportText);
            return reportText;
        }
    }
}
