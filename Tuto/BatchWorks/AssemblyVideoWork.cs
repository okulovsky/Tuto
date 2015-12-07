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
    public class AssemblyVideoWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades {get; set;}
        private List<AvsNode> episodes { get; set; }

        public AssemblyVideoWork (EditorModel model)
        {
            Model = model;
            crossFades = model.Videotheque.CrossFadesEnabled;
            Name = "Assembly video: " + model.Locations.FaceVideo.Directory.FullName;
            filesToDelIfAborted = new List<string>();
            var service = new AssemblerService(crossFades);
            episodes = service.GetEpisodesNodes(Model);

            for (var episodeNumber = 0; episodeNumber < episodes.Count; episodeNumber++)
            {
                if (Model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.None)
                    continue;
                var episodeInfo = model.Montage.Information.Episodes[episodeNumber];
                BeforeWorks.Add(new AssemblyEpisodeWork(model,episodeInfo));
            }
            var test = 5;
        }
    }
}
