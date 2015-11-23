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
            var episodeNumber = 0;
            foreach (var episode in episodes)
            {
                var from = model.Locations.GetOutputFile(episodeNumber);
                var to = new FileInfo(Path.Combine(model.Videotheque.OutputFolder.FullName, from.Name)); 
                AfterWorks.Add(new MoveFile(from, to));
                AfterWorks.Add(new YoutubeWork(model, episodeNumber, to));
                episodeNumber++;
            }
        }
    }
}
