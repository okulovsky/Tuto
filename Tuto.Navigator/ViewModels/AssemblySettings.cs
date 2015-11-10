using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tuto.BatchWorks;
using Tuto.Model;

namespace Tuto.Navigator.ViewModels
{
    public class AssemblySettings
    {
        //Refactoring!
        public bool CleanSound { get; set; }

        public bool FaceThumb { get; set; }
        public bool DesktopThumb { get; set; }
        public bool ConvertNeeded { get; set; }

        public bool RepairFace { get; set; }
        public bool RepairDesktop { get; set; }

        public bool AssemblyNeeded { get; set; }
        public bool All { get; set; }

        public List<BatchWork> GetWorksAccordingSettings(EditorModel m)
        {
            var tasks = new List<BatchWork>();
            if (!All)
            {
                if (RepairDesktop)
                    tasks.Add(new RepairVideoWork(m, m.Locations.DesktopVideo));
                if (RepairFace)
                    tasks.Add(new RepairVideoWork(m, m.Locations.FaceVideo));
                if (FaceThumb)
                    tasks.Add(new CreateThumbWork(m.Locations.FaceVideo, m));
                if (DesktopThumb)
                    tasks.Add(new CreateThumbWork(m.Locations.DesktopVideo, m));
                if (ConvertNeeded)
                {
                    tasks.Add(new ConvertVideoWork(m, m.Locations.DesktopVideo, false));
                    tasks.Add(new ConvertVideoWork(m, m.Locations.FaceVideo, true));
                }
                if (CleanSound)
                    tasks.Add(new CreateCleanSoundWork(m.Locations.FaceVideo, m));

                if (AssemblyNeeded)
                    tasks.Add(new AssemblyVideoWork(m, m.Global.CrossFadesEnabled));
            }
            else tasks = GetOptionsAccordingAllOption(m);
            return tasks;
        }

        private List<BatchWork> GetOptionsAccordingAllOption(EditorModel m)
        {
            var tasks = new List<BatchWork>();
            if (m.Global.WorkSettings.DesktopThumbSettings.CurrentOption != Options.Skip)
                tasks.Add(new CreateThumbWork(m.Locations.DesktopVideo, m));
            if (m.Global.WorkSettings.FaceThumbSettings.CurrentOption != Options.Skip)
                tasks.Add(new CreateThumbWork(m.Locations.FaceVideo, m));
            tasks.Add(new AssemblyVideoWork(m, m.Global.CrossFadesEnabled));
            return tasks;
        }
    }
}
