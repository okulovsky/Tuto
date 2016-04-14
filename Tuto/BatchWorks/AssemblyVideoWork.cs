using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using Tuto.TutoServices;
using Tuto.TutoServices.Assembler;

namespace Tuto.BatchWorks
{
    public class AssemblyVideoWork : CompositeWork
    {
        public AssemblyVideoWork (EditorModel model)
        {
            Model = model;
            Name = "Assembly video: " + model.RawLocation.Name;
            var service = new AssemblerService(model.Videotheque.Data.EditorSettings.CrossFadesEnabled);
            var episodes = service.GetEpisodesNodes(Model);

            for (var episodeNumber = 0; episodeNumber < episodes.Count; episodeNumber++)
            {
                if (Model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.None)
                    continue;
                var episodeInfo = model.Montage.Information.Episodes[episodeNumber];
                Tasks.Add(new AssemblyEpisodeWork(model,episodeInfo));
            }
        }
    }
}
