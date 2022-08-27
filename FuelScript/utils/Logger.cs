using System;
using System.IO;
using System.Windows.Forms;

namespace FuelScript.utils
{
    public class Logger
    {
        private static readonly string INFO_PREFIX = "INFO";
        private static readonly string WARN_PREFIX = "WARN";
        private static readonly string ERROR_PREFIX = "ERROR";
        private static readonly string DEBUG_PREFIX = "DEBUG";
        private readonly bool DEBUG = true;
        private readonly string script;
        private readonly string filePath;

        public Logger(string script, bool debug) : this(script)
        {
            DEBUG = debug;
        }

        public Logger(string script)
        {
            this.script = script;
            filePath = Application.StartupPath + "\\scripts\\logs\\" + script + ".log";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            try
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    // si es mayor a un mega lo creo de nuevo
                    if (fileInfo.Length / (1024 * 1024) > 1)
                    {
                        File.Delete(filePath);
                        new FileStream(filePath, FileMode.Create);
                    }
                }
            }
            catch { }
        }

        public void Info(string method, string message)
        {
            DoLog(INFO_PREFIX, message, method);
        }

        public void Info(string message)
        {
            DoLog(INFO_PREFIX, message, null);
        }

        public void Warn(string method, string message)
        {
            DoLog(WARN_PREFIX, message, method);
        }

        public void Warn(string message)
        {
            DoLog(WARN_PREFIX, message, null);
        }

        public void Error(string method, string message)
        {
            DoLog(ERROR_PREFIX, message, method);
        }

        public void Error(string message)
        {
            DoLog(ERROR_PREFIX, message, null);
        }


        public void Debug(string method, string message)
        {
            if (DEBUG)
            {
                DoLog(DEBUG_PREFIX, message, method);
            }
        }

        public void Debug(string message)
        {
            if (DEBUG)
            {
                DoLog(DEBUG_PREFIX, message, null);
            }
        }

        private void DoLog(string level, string message, string method)
        {
            method = method != null ? ": " + method : "";
            try
            {
                using (StreamWriter streamWriter = File.AppendText(filePath))
                {
                    streamWriter.Write(DateTime.Now.ToString("[" + "dd-MM-yyyy hh:mm:ss.fff tt") + "][" + level + "][" + script + method + "] " + message + "\n");
                    streamWriter.Close();
                }
            }
            catch { }
        }
    }
}

