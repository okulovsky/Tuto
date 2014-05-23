using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xspf2list
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("xspf2list <xspffile> <txtfile>");
                Console.ReadKey();
                return;
            }

         
            XDocument doc = XDocument.Load(args[0]);

            var relative = new FileInfo(args[0]).DirectoryName + "\\";

            var tracks = doc
                        .Elements()
                        .Where(z => z.Name.LocalName == "playlist")
                        .Elements()
                        .Where(z => z.Name.LocalName == "trackList")
                        .Elements()
                        .Select(z => z.Elements().Where(x => x.Name.LocalName == "location").FirstOrDefault())
                        .Select(z => z.Value)
                        .Select(z => z.Substring(8, z.Length - 8))
                        .Select(z => z.Replace("/", "\\"))
                        .Select(z => z.Substring(relative.Length, z.Length - relative.Length))
                        .Select(z => "file '" + z + "'")
                        .ToArray();

                        
                        
            foreach (var e in tracks)
                Console.WriteLine("{0}", e);

            File.WriteAllLines(args[1], tracks);
        }
    }
}
