using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;

namespace Tuto.Navigator.NewLook
{
    public class EditorViewModel : NotifierModel
    {
        public EditorModel EditorModel { get; private set; }

        public IEnumerable<EpisodeViewModel> EpisodesInfo
        {
            get
            {
                return EditorModel.Montage.Information.Episodes.Select(z => new EpisodeViewModel(z));
            }
        }

        public string DisplayedName { get { return EditorModel.Montage.DisplayedRawLocation; } }

        public Visibility FilesBroken { get { return (EditorModel.InputFilesAreOK)?Visibility.Collapsed:Visibility.Visible;  } }

        public Visibility VoiceNotReady
        {
            get
            {
                return (EditorModel.Montage.SoundIntervals != null
                    && EditorModel.Montage.SoundIntervals.Count > 3)
                    ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public EditorViewModel(EditorModel e)
        {
            EditorModel = e;
            e.EnvironmentChanged += e_EnvironmentChanged;
        }

        void e_EnvironmentChanged(object sender, EventArgs e)
        {
            base.NotifyAll();
        }
    }
}
