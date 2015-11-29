using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;
using Tuto.TutoServices;

namespace Tuto.BatchWorks
{
    //работа-метка, поэтому метода Work() нет
    public class MakeAll : BatchWork
    {
        public MakeAll(EditorModel model)
        {
            BeforeWorks.Add(new AssemblyVideoWork(model));

            var serv = new AssemblerService(true);
            var episodes = serv.GetEpisodesNodes(model);

            //с эпизодами надо что-то решить, в силу того, что эпизодинфо не знает нужной инфы, другого выхода не вижу
            //мы это уже обсуждали, но так и не пришли ни к чему ((

            for (int episodeNumber=0; episodeNumber < episodes.Count; episodeNumber++)
            {
                if (model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.None)
                    continue;

                if (model.Montage.Information.Episodes[episodeNumber].PatchModel != null)
                {
                    BeforeWorks.Add(new PatchWork(model.Montage.Information.Episodes[episodeNumber].PatchModel,
                                                  model.Videotheque.CrossFadesEnabled,
                                                  model,
                                                  true));
                    var patchedLocation = model.Locations.GetFinalOutputFile(episodeNumber);
                    AfterWorks.Add(new YoutubeWork(model, episodeNumber, patchedLocation));
                    continue;
                }

                var from = model.Locations.GetOutputFile(episodeNumber);
                var to = model.Locations.GetFinalOutputFile(episodeNumber); 

                if (model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.Patch)
                {
                    to = model.Locations.GetFinalPatchFile(episodeNumber);
                }

                AfterWorks.Add(new MoveFile(from, to));
                AfterWorks.Add(new YoutubeWork(model, episodeNumber, to));
            }
        }
    }
}
