using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportListeDeSuivi.util
{
    public interface ILogger
    {
        void Log(string msg, params object[] args);
        void Err(string msg, params object[] args);
    }

   public class ConsoleLogger : ILogger
    {

        public void Log(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        public void Err(string msg, params object[] args)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, args);
            Console.ForegroundColor = color;
        }
    }

   public class Filelogger : ILogger
    {
        public string Path { get; set; }

        public Filelogger(string path)
        {
            Path = path;
        }

        public void Log(string msg, params object[] args)
        {
            using (StreamWriter writer = File.AppendText(Path))
            {
                writer.WriteLine(String.Format("[OK]" + msg, args));
            }
        }

        public void Err(string msg, params object[] args)
        {
            using (StreamWriter writer = File.AppendText(Path))
            {
                writer.WriteLine(String.Format("[ERR] " + msg, args));
            }
        }
    }

    public static class Logger
    {
        private delegate void LoggerDelegate(string msg, params object[] args);
        private static event LoggerDelegate onLog;
        private static event LoggerDelegate onErr;
        public static int ErrorCount { get; set; }

        public static void Log(string msg, params object[] args)
        {
            if (onLog != null) onLog(msg, args);
        }

        public static void Err(string msg, params object[] args)
        {
            ErrorCount++;
            if (onLog != null) onErr(msg, args);
        }

        public static void Subsribe(ILogger logger)
        {
            onLog += new LoggerDelegate(logger.Log);
            onErr += new LoggerDelegate(logger.Err);
        }
    }
}
 
