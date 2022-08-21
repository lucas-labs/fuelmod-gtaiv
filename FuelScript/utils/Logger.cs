using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FuelScript.utils {
    public static class Log {
        private static readonly string INFO_PREFIX = "INFO";
        private static readonly string WARN_PREFIX = "WARN";
        private static readonly string ERROR_PREFIX = "ERROR";
        private static readonly string DEBUG_PREFIX = "DEBUG";

        private static readonly bool DEBUG = true;

        public static bool isDebug() {
            return DEBUG;
        }

        public static void info(string message) {
            log(INFO_PREFIX, message);
        }

        public static void warn(string message) {
            log(WARN_PREFIX, message);
        }

        public static void error(string message) {
            log(ERROR_PREFIX, message);
        }

        public static void debug(string message) {
            if (DEBUG) {
                log(DEBUG_PREFIX, message);
            }
        }

        private static void log(string level, string message) {
            try {
                using (StreamWriter streamWriter = File.AppendText(Application.StartupPath + "\\scripts\\CarOwner.log")) {
                    streamWriter.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff tt") + ": " + level + " - " + message);
                    streamWriter.Close();
                }
            } catch { }
        }
    }
}

