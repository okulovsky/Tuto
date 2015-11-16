using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto
{
    public static class Shell
    {
        public static bool SilentMode = false;

        public static void ExecQuoteArgs(bool print, FileInfo executable, params string[] args)
        {
            var argsLine = args.Select(z => "\"" + z + "\"").Aggregate((a, b) => a + " " + b);
            Exec(print, executable, argsLine);
        }

        public static void Exec(bool print, string dir, string command, string args)
        {
            var fullPath = Path.Combine(dir, command);
            if (print)
            {
                Console.WriteLine("{0} {1}", fullPath, args);
            }
            else
            {
                var process = new Process();
                process.StartInfo.FileName = fullPath;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = !SilentMode;
                process.StartInfo.CreateNoWindow = SilentMode;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new ApplicationException(
                        string.Format(
                            "Application returned an error code.\nApplication: {0}\nArguments:   {1}\nError code:  {2}\nCommand line to check:\n\"{0}\" {1}",
                            fullPath,
                            args,
                            process.ExitCode));
            }
        }

        public static void Exec(bool print, FileInfo executable, string args)
        {
            Exec(print, "", executable.FullName, args);
        }

        public static void Exec(bool print, FileInfo executable, string argumentFormat, params object[] arguments)
        {
             Exec(print, executable, string.Format(argumentFormat, arguments));
        }

        public static void Cmd(bool print, string argumentFormat, params object[] arguments)
        {
             Exec(print, Environment.SystemDirectory,"cmd", string.Format(argumentFormat,arguments));
        }

        public static void FFMPEG(bool print, string argumentFormat, params object[] arguments)
        {
             Exec(print, @"C:\ffmpeg\bin", "ffmpeg", string.Format(argumentFormat, arguments));
        }
    }
}
