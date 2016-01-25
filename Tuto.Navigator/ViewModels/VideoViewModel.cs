using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Tuto.Model;
using System.Linq;
using System.Diagnostics;
using Editor;
using Tuto.BatchWorks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Tuto.Navigator.ViewModels
{


    public class VideoViewModel : NotifierModel
    {
        public EditorModel Model { get; private set; }

        public event Action<VideoViewModel> OpenMe;
        public event Action Back;

        #region Displayed data
        public string Name
        {
            get
            {
                if (Model != null) return Model.Montage.DisplayedRawLocation;
                return "Hackerdom\\Lecture09\\01";
            }
        }

        public IEnumerable<EpisodInfo> Episodes
        {
            get
            {
				if (Model != null)
				{
					if (Model.Montage.Information != null)
						return Model.Montage.Information.Episodes;
					return null;
				}
				return new[]
				{
					new EpisodInfo(Guid.NewGuid()) { Name="Экономика биткоинов", OutputType = OutputTypes.Output },
					new EpisodInfo(Guid.NewGuid()) { Name="Патч", OutputType = OutputTypes.Patch},
					new EpisodInfo(Guid.NewGuid()) { Name="Факап", OutputType = OutputTypes.None }
				};
            }
        }

        public int? TotalDuration
        {
            get
            {
                if (Model != null)
                {
                    if (Model.Montage.Information != null)
                        return (int)Model.Montage.Information.Episodes.Sum(z => z.Duration.TotalMinutes);
                    return null;
                }
                return 17;
            }
        }

        public string ModificationDate
        {
            get
            {
                if (Model != null)
                    return string.Format("{0:dd.MM.yy hh:mm}", Model.Montage.Information.LastModificationTime);
                return "23.12.15 16:00";
            }
        }

        #endregion 

        #region Commands
        public RelayCommand Edit { get; private set; }
        public RelayCommand OpenSource { get; private set; }
        public RelayCommand OpenTemp { get; private set; }

        public RelayCommand MakeAll { get; private set; }

        public RelayCommand BackToNavigator { get; private set; }

        #endregion

        #region Controlled properties

        bool selected;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; NotifyPropertyChanged(); }
        }

        #endregion


        internal VideoViewModel() { }

        public VideoViewModel(EditorModel model)
        {
            this.Model = model;

            Edit = new RelayCommand(
                () =>
                {
                    if (OpenMe != null)
                        OpenMe(this);
                });

            OpenSource = new RelayCommand(
                ()=>Process.Start(Model.Locations.FaceVideo.Directory.FullName),
                ()=>Model != null && Model.Locations.FaceVideo.Exists);

            OpenTemp = new RelayCommand(
                ()=>Process.Start(Model.Locations.TemporalDirectory.FullName),
                () => Model != null && Model.Locations.TemporalDirectory.Exists
                );

            MakeAll = new RelayCommand(
                () => { Program.WorkQueue.Run(new MakeAll(Model)); },
                ()=>Model.Statuses.SourceIsPresent
                );

            BackToNavigator = new RelayCommand(
                () => { if (Back != null) Back(); }
            );
        }
       


      public  IEnumerable<string> GetTextInfo()
        {
		  if (Model.Montage.DisplayedRawLocation!=null)
            yield return Model.Montage.DisplayedRawLocation;
            if (Model.Montage.Information != null)
                foreach (var e in Model.Montage.Information.Episodes)
                    yield return e.Name;
        }
    }
} 