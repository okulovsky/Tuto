using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class Statuses : NotifierModel
    {
        readonly EditorModel model;
        public Statuses(EditorModel model)
        {
            this.model = model;
        }

        public bool VoiceAnalyzed
        {
            get
            {
                return model.Montage.SoundIntervals != null && model.Montage.SoundIntervals.Count > 3;
            }
        }

        public bool VoiceIsNotAnalyzed
        {
            get
            {
                return !VoiceAnalyzed;
            }
        }

        public bool SourceIsPresent
        {
            get
            {
                return model.Locations.FaceVideo.Exists;
            }
        }

        public bool SourceIsNotPresent
        {
            get
            {
                return !SourceIsPresent;
            }
        }

        public void Update()
        {
            base.NotifyAll();
        }


    }
}
