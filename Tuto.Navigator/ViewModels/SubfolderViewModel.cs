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

namespace Tuto.Navigator
{

    public class SubfolderViewModel : INotifyPropertyChanged
    {
        public Action<IEnumerable<BatchWork>> addTaskToQueue;
        private EditorModel model { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public SubfolderViewModel(EditorModel model)
        {
            this.model = model;
            StartEditorCommand = new RelayCommand(StartEditor);
            ResetMontageCommand = new RelayCommand(ResetMontage);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            FullPath = model.Locations.LocalFilePath.Directory.FullName;
            if (model.Montage.Chunks != null && model.Montage.Chunks.Count > 3)
                Marked = true;

            if (model.Montage.Information != null && model.Montage.Information.Episodes.Count>0)
            {
                TotalDuration = model.Montage.Information.Episodes.Sum(z => z.Duration.TotalMinutes);
                int index = 0;
                EpisodesNames = model.Montage.Information.Episodes
                    .Select(z => 
                    {
                        var info = new EpisodeBindingInfo();
                        info.Title = z.Name;
                        info.FullName = Path.Combine(
                    model.Locations.OutputDirectory.FullName,
                    string.Format("{0}-{1} {2}.avi",
                        model.VideoFolder.Name,
                        index++,
                        z.Name));
                        info.Guid = z.Guid;
                        return info;
                    }).ToList();
            }
        }
        public bool Selected { get; set; }

        public bool Marked { get; private set; }

        private bool _readyToEdit;
        public bool ReadyToEdit
        {
            get { return _readyToEdit; }
            set { _readyToEdit = value; OnPropertyChanged("ReadyToEdit"); }
        }

        public string FullPath { get; private set; }

        public string Name {get { return Path.GetFileName(FullPath); }}

        public List<EpisodeBindingInfo> EpisodesNames { get; private set; }

        public double? TotalDuration { get; private set; }

        public void StartEditor()
        {

            var model = EditorModelIO.Load(EditorModelIO.SubstituteDebugDirectories(FullPath));
            var window = new MainEditorWindow();
            window.addTaskToQueue = addTaskToQueue;
            window.DataContext = model;
            window.Show();
        }

        public RelayCommand StartEditorCommand { get; private set; }

        public void ResetMontage()
        {
            var ok = MessageBox.Show("This action only makes sense if you corrected an internal error in Tuto. Have you done it?", "Tuto.Navigator", MessageBoxButtons.YesNoCancel);
            if (ok != DialogResult.Yes) return;
            var model = EditorModelIO.Load(FullPath);
            model.Montage.ReadyToEdit = false;
            EditorModelIO.Save(model);
        }

        public RelayCommand ResetMontageCommand { get; private set; }

        public void OpenFolder()
        {
            Process.Start(FullPath);
        }

        public RelayCommand OpenFolderCommand { get; private set; }
    }
} 