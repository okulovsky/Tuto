using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.BatchWorks;

namespace Tuto.Model
{
    public class PreparingSettings
    {
        private EditorModel model;
        public bool NecessaryCleanVoice { get; set; }
        public bool ConvertVideos { get; set; }
        public bool CreateThumbs { get; set; }

        public IEnumerable<BatchWork> GetWorks(EditorModel model)
        {
            var tasks = new List<BatchWork>();
            if (!model.Locations.PraatVoice.Exists)
                tasks.Add(new PraatWork(model));

            if (NecessaryCleanVoice && !model.Locations.ClearedSound.Exists)
                tasks.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model));

            if (ConvertVideos && !model.Locations.ConvertedFaceVideo.Exists)
                tasks.Add(new ConvertFaceVideoWork(model));

            if (ConvertVideos && !model.Locations.ConvertedDesktopVideo.Exists)
                tasks.Add(new ConvertDesktopVideoWork(model));

            if (CreateThumbs && !model.Locations.DesktopVideoThumb.Exists)
                tasks.Add(new CreateThumbWork(model.Locations.DesktopVideo, model));

            if (CreateThumbs && !model.Locations.FaceVideoThumb.Exists)
                tasks.Add(new CreateThumbWork(model.Locations.FaceVideo, model));

            return tasks;

        }
    }
}
