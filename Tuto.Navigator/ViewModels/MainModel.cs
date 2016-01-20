using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator.ViewModels;

namespace Tuto.Navigator.ViewModels
{

    public enum MainMode
    {
        Videotheque,
        Video
    }

    public class MainModel : NotifierModel
    {
        public BatchWorkQueueViewModel Queue { get; private set; }
        public VideothequeModel VideothequeModel { get; private set; }

        public EditorModel CurrentVideo { get; private set; }

        
        MainMode mode;
        public MainMode Mode 
        { 
            get { return mode;}
            set 
            {
                mode=value;
                NotifyAll();
            }
        }

        public bool VideothequeVisible { get { return mode == MainMode.Videotheque;  } }
        public bool EditorVisible { get { return mode == MainMode.Video; } }

        public MainModel(Videotheque videotheque)
        {
            Queue = new BatchWorkQueueViewModel(Program.WorkQueue);
            VideothequeModel = new VideothequeModel(videotheque);
            VideothequeModel.OpenEditor += OpenEditor;
            Mode = MainMode.Videotheque;
        }

        void OpenEditor(VideoViewModel obj)
        {
            CurrentVideo = obj.Model;
            obj.Model.WindowState.GetBack+=BackToNavigator;
            Mode = MainMode.Video;
        }

        void BackToNavigator()
        {
            CurrentVideo = null;
            Mode = MainMode.Videotheque;
        }


    }
}
