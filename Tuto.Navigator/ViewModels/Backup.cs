using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator.ViewModels
{
    class Backup
    {
        public static string CreateBackup(Videotheque global, List<EditorModel> models)
        {
            var glob=global.GlobalDataFolder.FullName;
            var builder = new StringBuilder();
            builder.AppendFormat("xcopy \"{0}\\{1}\" .\\\r\n", glob, Locations.GlobalFileName);
            builder.AppendFormat("xcopy \"{0}\\{1}\" .\\\r\n", glob, Locations.PublishingFileName);
            foreach (var e in models)
            {
                var relative = global.Locations.RelativeToGlobal(e.VideoFolder.FullName);
                builder.AppendFormat("mkdir \"{0}\"\r\n", relative);
                var format="xcopy \"{0}{1}\\{2}\" \".{1}\\\"\r\n";
                builder.AppendFormat(format, glob, relative, Locations.FaceVideoFileName);
                builder.AppendFormat(format, glob, relative, Locations.DesktopVideoFileName);
                builder.AppendFormat(format, glob, relative, Locations.LocalFileName);
            }
            return builder.ToString();
        }
    }
}
