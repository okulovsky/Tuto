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
    public class AssemblyEpisodeWork : CompositeWork // composite work
    {
        public AssemblyEpisodeWork(EditorModel model, EpisodInfo episodeInfo)
        {
            Model = model;
            var videoFile = model.Locations.GetOutputFile(episodeInfo);
            Tasks.Add(new ConvertDesktopWork(model, false));
            Tasks.Add(new ConvertFaceWork(model, false));
            Tasks.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model, false));
            Tasks.Add(new AtomicAssemblyEpisodeWork(model,episodeInfo)); // this work itself

            var task = new NormalizeSoundWork(Model, videoFile);
            task.Name = "Normalize audio: " + episodeInfo.Name;
            Name = "Assembly episode: " + episodeInfo.Name;
            Tasks.Add(task);
        }
    }
}
