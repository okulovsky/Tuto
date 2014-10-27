using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    class MoveChunksToTempFiles
    {
        public void Run()
        {
            var allData = Tuto.Model.EditorModelIO.ReadAllProjectData(
              new System.IO.DirectoryInfo("D:\\BP"));
            var result=new List<string>();
            foreach (var e in allData.Models)
            {
                var parent=e.Locations.TemporalDirectory.Parent.FullName;
                result.Add(string.Format(@"rd ""{0}"" /Q",e.Locations.TemporalDirectory.FullName));
                result.Add(string.Format(@"move ""{0}"" ""{1}""",e.VideoFolder+"\\chunks",parent));
                result.Add(string.Format(@"rename ""{0}"" ""{1}""", parent+"\\chunks",e.Locations.TemporalDirectory.Name));
            }
            File.WriteAllLines("D:\\BP\\moveChunks.bat",result.ToArray());
        }
    }
}
