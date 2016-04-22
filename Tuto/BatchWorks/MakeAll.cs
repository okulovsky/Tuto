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
    public class MakeAll : CompositeWork
    {
        public MakeAll(EditorModel model)
        {
            Name = "Make all for: " + model.RawLocation.Name;
            var videoWork = new AssemblyVideoWork(model);
            Tasks.Add(videoWork);
            var serv = new AssemblerService(true);
            var episodes = serv.GetEpisodesNodes(model);

            //с эпизодами надо что-то решить, в силу того, что эпизодинфо не знает нужной инфы, другого выхода не вижу
            //мы это уже обсуждали, но так и не пришли ни к чему ((

            for (int episodeNumber=0; episodeNumber < episodes.Count; episodeNumber++)
            {
                if (model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.None)
                    continue;

                //if (model.Montage.Information.Episodes[episodeNumber].PatchModel != null)
                //{
                //    BeforeWorks.Add(new PatchWork(model.Montage.Information.Episodes[episodeNumber].PatchModel,
                //                                  model.Videotheque.Data.EditorSettings.CrossFadesEnabled,
                //                                  model,
                //                                  true));
                //    var patchedLocation = model.Locations.GetFinalOutputFile(episodeNumber);
                //    AfterWorks.Add(new YoutubeWork(model, episodeNumber, patchedLocation));
                //    continue;
                //}

                var from = model.Locations.GetOutputFile(episodeNumber);
                var to = model.Locations.GetFinalOutputFile(episodeNumber);

                if (model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.Patch)
                    continue;

                
                var task = new MoveFile(from, to, model);
                var youtubeWork = new YoutubeWork(model, episodeNumber, false);

                videoWork.Tasks.Select(x => x as CompositeWork).ElementAt(episodeNumber).Tasks.Add(task);
                videoWork.Tasks.Select(x => x as CompositeWork).ElementAt(episodeNumber).Tasks.Add(youtubeWork);
            }
        }
    }
}
