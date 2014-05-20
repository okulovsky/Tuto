using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Renamer [folder]");
                return;
            }

            var folderName = new DirectoryInfo(args[0]).Name;

            var names = File.ReadAllLines(args[0] + "\\titles.txt");
            for (int i = 0; i < names.Length; i++)
            {
                File.Move(
                    string.Format("{0}\\result-{1}.avi", args[0], i),
                    string.Format("{0}\\AIML{1}{2} {3}.avi", args[0], folderName, i + 1, names[i]));
            }
        }
    }
}
