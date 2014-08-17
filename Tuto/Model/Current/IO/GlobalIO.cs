using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static partial class EditorModelIO
    {

        const string GlobalHeader = "Tuto project file";

        public static GlobalData ReadGlobalData(DirectoryInfo rootFolder)
        {
            var file = rootFolder.GetFiles(Locations.GlobalFileName).FirstOrDefault();
            if (file == null)
                return new GlobalData { GlobalDataFolder = rootFolder };
            var data = HeadedJsonFormat.Read<GlobalData>(file, GlobalHeader,0 );
            if (data.VideoData == null) data.VideoData = new List<FinishedVideo>();
            if (data.TopicsRoot == null) data.TopicsRoot = new Topic();
            data.AfterLoad(rootFolder);
            return data;
        }

        public static AllProjectData ReadAllProjectData(DirectoryInfo rootFolder)
        {
            var globalData = ReadGlobalData(rootFolder);

            var result = new AllProjectData(globalData);

            var dirs = rootFolder
                       .GetDirectories()
                       .Where(dir => dir.Name != "Output") //очень грубый костыль
                       .OrderByDescending(z => z.CreationTime);

            foreach (var e in dirs)
            {
                var model = EditorModelIO.Load(e.FullName);
                result.Models.Add(model);
                for (int i = 0; i < model.Montage.Information.Episodes.Count; i++)
                {
                    var v = model.Montage.Information.Episodes[i];
                    var ex = globalData.VideoData.Where(z => z.Guid == v.Guid).FirstOrDefault();
                    if (ex == null)
                        globalData.VideoData.Add(new FinishedVideo(model, i));
                    else
                        ex.Load(model, i);
                }
            }
            return result;

        }

        public static void Save(GlobalData data)
        {
            HeadedJsonFormat.Write<GlobalData>(data.Locations.ProjectFile, "Tuto project file", 0, data);
        }
    }
}