using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    class Statistics
    {
        public void Run()
        {
            var allData = Tuto.Model.EditorModelIO.ReadAllProjectData(
                new System.IO.DirectoryInfo("D:\\BP"));
            var percents = new List<double>();
            foreach (var e in allData.Models)
            {
                var total=0;
                var good=0;
                foreach (var c in e.Montage.Chunks)
                {
                    if (c.Mode == Editor.Mode.Undefined) continue;
                    total += c.Length;
                    if (c.Mode == Editor.Mode.Drop) continue;
                    good += c.Length;
                }
                percents.Add((100.0 * good) / total);
            }
            Console.WriteLine(percents.OrderBy(z => z).Skip(10).Average());

        }
    }
}
