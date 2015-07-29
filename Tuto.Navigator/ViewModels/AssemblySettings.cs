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
        public bool GenerateWithFades { get; set; }
        public bool GenerateWithoutFades { get; set; }
        public bool CleanSound { get; set; }

        public List<BatchWork> GetWorksAccordingSettings(EditorModel m)
        {
            var tasks = new List<BatchWork>();
            if (CleanSound)
                tasks.Add(new CreateCleanSoundWork(m.Locations.FaceVideo, m));
            if (GenerateWithFades)
                tasks.Add(new AssemblyVideoWork(m, true));
            if (GenerateWithoutFades)
                tasks.Add(new AssemblyVideoWork(m, false));
            return tasks;
        }
    }
}
