using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tuto.BatchWorks;
using Tuto.Model;

namespace Tuto.Navigator.ViewModels
{
    public class AssemblySettings
    {
        //Refactoring!

        public bool FaceThumb { get; set; }
        public bool DesktopThumb { get; set; }
        public bool ConvertNeeded { get; set; }

        public bool RepairFace { get; set; }
        public bool RepairDesktop { get; set; }

        public bool AssemblyNeeded { get; set; }
        public bool CleanSound { get; set; }
        public bool UploadSelected { get; set; }
        
        public bool All { get; set; }

        public List<BatchWork> GetWorksAccordingSettings(EditorModel m)
        {
            var tasks = new List<BatchWork>();
            if (!All)
            {
                if (RepairDesktop)
                    tasks.Add(new RepairVideoWork(m, m.Locations.DesktopVideo, true));
                if (RepairFace)
                    tasks.Add(new RepairVideoWork(m, m.Locations.FaceVideo, true));
                if (FaceThumb)
                    tasks.Add(new CreateThumbWork(m.Locations.FaceVideo, m, true));
                if (DesktopThumb)
                    tasks.Add(new CreateThumbWork(m.Locations.DesktopVideo, m, true));
                if (ConvertNeeded)
                {
                    tasks.Add(new ConvertDesktopWork(m, true));
                    tasks.Add(new ConvertFaceWork(m, true));
                }

                if (CleanSound)
                    tasks.Add(new CreateCleanSoundWork(m.Locations.FaceVideo, m, true));
                if (AssemblyNeeded)
                    tasks.Add(new AssemblyVideoWork(m));

                if (UploadSelected)
                {
                    for (var i = 0; i < m.Montage.Information.Episodes.Count; i++)
                    {
                        var episode = m.Locations.GetFinalOutputFile(i);
                        tasks.Add(new YoutubeWork(m, i, episode));
                    }
                }
                foreach (var e in tasks)
                    e.Forced = true;
            }
            else tasks = GetOptionsAccordingAllOption(m);
            return tasks;
        }

        private List<BatchWork> GetOptionsAccordingAllOption(EditorModel m)
        {
            var tasks = new List<BatchWork>();
            if (m.Videotheque.Data.WorkSettings.DesktopThumbSettings.CurrentOption != Options.Skip)
                tasks.Add(new CreateThumbWork(m.Locations.DesktopVideo, m, true));
            if (m.Videotheque.Data.WorkSettings.FaceThumbSettings.CurrentOption != Options.Skip)
                tasks.Add(new CreateThumbWork(m.Locations.FaceVideo, m, true));
            tasks.Add(new AssemblyVideoWork(m));
            return tasks;
        }
    }
}
