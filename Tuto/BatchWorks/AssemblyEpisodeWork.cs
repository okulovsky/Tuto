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
            var convertDesktop = new ConvertDesktopWork(model, false);
            var convertFace = new ConvertFaceWork(model, false);
            var createSoundWork = new CreateCleanSoundWork(model.Locations.FaceVideo, model, false);
            convertDesktop.Name = "Converting Desktop video";
            convertFace.Name = "Converting Face video";
            createSoundWork.Name = "Cleaning sound";

            if (model.Locations.DesktopVideo.Exists)
                Tasks.Add(convertDesktop);

            if (model.Locations.FaceVideo.Exists)
                Tasks.Add(convertFace);

            Tasks.Add(createSoundWork);
            Tasks.Add(new AtomicAssemblyEpisodeWork(model,episodeInfo)); // atomic version

            var task = new NormalizeSoundWork(Model, videoFile);
            Name = "Assembling episode: " + episodeInfo.Name;
            Tasks.Add(task);
        }
    }
}
