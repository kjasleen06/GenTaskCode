using System;
using System.IO;

namespace ClassLibrary1
{
    internal static class Log
    {
        private static string logFile = "Log.txt";
        private static string filePath;

        public static void SetupLogFile()
        {
            try
            {
                filePath = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName, $"{logFile}");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Issue in SetupLogFile: " + ex.ToString());
            }
        }
        public static void Debug(string logMessage)
        {
            try
            {
                using (StreamWriter w = File.AppendText(filePath))
                {
                    var syntax = $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}" + ":" + logMessage;
                    w.WriteLine(syntax);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Issue in writing the message in log file: " + ex.ToString());
            }
        }
    }
}
