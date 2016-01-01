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

namespace Tuto.Navigator
{


    public class SubfolderViewModel : INotifyPropertyChanged
    {
        public EditorModel Model { get; private set; }

        #region Displayed data
        public string Name
        {
            get
            {
                if (Model != null) return Model.Montage.DisplayedRawLocation;
                return "Hackerdom\\Lecture09\\01";
            }
        }

        public IEnumerable<string> EpisodesNames
        {
            get
            {
                if (Model != null)
                    if (Model.Montage.Information != null)
                        return Model.Montage.Information.Episodes.Select(z => z.Name);
                return new[] { "јрхитектура биткоинов", "Ёкономика биткоинов" };
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
                    return string.Format("{0:dd.MM.yy hh:mm}", Model.Montage.ModificationTime);
                return "23.12.15 16:00";
            }
        }

        #endregion 

        #region Commands
        public RelayCommand Edit { get; private set; }
        public RelayCommand OpenSource { get; private set; }
        public RelayCommand OpenTemp { get; private set; }


        #endregion

        


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal SubfolderViewModel() { }

        public SubfolderViewModel(EditorModel model)
        {
            this.Model = model;

            Edit = new RelayCommand(
                () =>
                {
                    var window = new MainEditorWindow();
                    window.DataContext = Model;
                    window.Show();
                });

            OpenSource = new RelayCommand(
                ()=>Process.Start(Model.Locations.FaceVideo.Directory.FullName),
                ()=>Model != null && Model.Locations.FaceVideo.Exists);

            OpenTemp = new RelayCommand(
                ()=>Process.Start(Model.Locations.TemporalDirectory.FullName),
                () => Model != null && Model.Locations.TemporalDirectory.Exists
                );
            
        }
        public bool Selected { get; set; }



      public  IEnumerable<string> GetTextInfo()
        {
            yield return Model.Montage.DisplayedRawLocation;
            if (Model.Montage.Information != null)
                foreach (var e in Model.Montage.Information.Episodes)
                    yield return e.Name;
        }
    }
} 