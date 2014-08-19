using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Tuto.Model;
using System.Linq;
using System.Diagnostics;

namespace Tuto.Navigator
{

    public class SubfolderViewModel
    {
        public SubfolderViewModel(EditorModel model)
        {
            
            StartEditorCommand = new RelayCommand(StartEditor);
            ResetMontageCommand = new RelayCommand(ResetMontage);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            FullPath = model.Locations.LocalFilePath.Directory.FullName;
            if (model.Montage.Chunks != null && model.Montage.Chunks.Count > 3)
                Marked = true;

            if (model.Montage.Information != null && model.Montage.Information.Episodes.Count>0)
            {
                TotalDuration = model.Montage.Information.Episodes.Sum(z => z.Duration.TotalMinutes);
                EpisodesNames = model.Montage.Information.Episodes
                    .Select(z=>z.Name)
                    .Aggregate((a, b) => a + "\r\n" + b);
            }
            if (model.Montage.Montaged)
                Montaged = true;
        }
        public bool Selected { get; set; }

        public bool Marked { get; private set; }

        public bool Montaged { get; private set; }

        public string FullPath { get; private set; }

        public string Name {get { return Path.GetFileName(FullPath); }}

        public string EpisodesNames { get; private set; }

        public double? TotalDuration { get; private set; }

        public void StartEditor()
        {
	        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var editorExe = new FileInfo(Path.Combine(path, "editor.exe"));
            Shell.ExecQuoteArgs(false, editorExe, FullPath);
        }

        public RelayCommand StartEditorCommand { get; private set; }

        public void ResetMontage()
        {
            var ok = MessageBox.Show("This action only makes sense if you corrected an internal error in Tuto. Have you done it?", "Tuto.Navigator", MessageBoxButtons.YesNoCancel);
            if (ok != DialogResult.Yes) return;
            var model = EditorModelIO.Load(FullPath);
            model.Montage.Montaged = false;
            EditorModelIO.Save(model);
            Montaged = false;
        }

        public RelayCommand ResetMontageCommand { get; private set; }

        public void OpenFolder()
        {
            Process.Start(FullPath);
        }

        public RelayCommand OpenFolderCommand { get; private set; }
    }
} 