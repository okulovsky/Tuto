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
                tasks.Add(new ConvertDesktopVideoWork(m));
                tasks.Add(new ConvertFaceVideoWork(m));
            }
            if (CleanSound)
                tasks.Add(new CreateCleanSoundWork(m.Locations.FaceVideo, m));

            if (AssemblyNeeded)
                tasks.Add(new AssemblyVideoWork(m, m.Global.CrossFadesEnabled));
            return tasks;
        }
    }
}
