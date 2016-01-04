using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace TimeByGitUpdater
{
    class Program
    {
        static DateTime[] StartGit(DirectoryInfo rep, FileInfo file)
        {
            var process = new Process();
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments=" log --format=\"%aD\" -- "+file.Name;
            process.StartInfo.WorkingDirectory = rep.FullName;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            Console.WriteLine(file.Name);
            List<DateTime> times = new List<DateTime>();
            while(true)
            {
                var str = process.StandardOutput.ReadLine();
                if (str == null) break;
                var time = DateTime.Parse(str);
                times.Add(time);
                Console.WriteLine(str);
                
            }
            Console.WriteLine();
            return times.ToArray();
        }

        static void Main(string[] args)
        {
            const string Path = @"C:\Users\Yura\Desktop\TestMontage\Models";
            var dir = new DirectoryInfo(Path);
            foreach(var e in dir.GetFiles())
            {
                FileContainer model = null;
                try
                {
                    model = HeadedJsonFormat.Read<FileContainer>(e);
                }
                catch
                {
                    continue;
                }
                var times = StartGit(dir, e);
                model.MontageModel.Information.LastModificationTime = times.First();
                model.MontageModel.Information.CreationTime = times.Last();
                HeadedJsonFormat.Write<FileContainer>(e, model);
            }
        }
    }
}
