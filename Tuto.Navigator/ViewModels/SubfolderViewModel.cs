using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Tuto.Model;
using System.Linq;

namespace Tuto.Navigator
{
    public class SubfolderViewModel
    {
        public SubfolderViewModel(string fullPath)
        {
            FullPath = fullPath;
            StartEditorCommand = new RelayCommand(StartEditor);
            ResetMontageCommand = new RelayCommand(ResetMontage);
            var model = EditorModelIO.Load(fullPath);
            Marked = model.Montage.Chunks != null && model.Montage.Chunks.Count > 3;

            if (model.Montage.Information != null && model.Montage.Information.Episodes.Count>0)
            {
                TotalDuration = model.Montage.Information.Episodes.Sum(z => z.Duration.TotalMinutes);
                EpisodesNames = model.Montage.Information.Episodes
                    .Select(z=>z.Name)
                    .Aggregate((a, b) => a + "\r\n" + b);
            }

            Montaged = model.Montage.Montaged;
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

        public void ResetMontage()
        {
            var ok = MessageBox.Show("This action only makes sense if you corrected an internal error in Tuto. Have you done it?", "Tuto.Navigator", MessageBoxButtons.YesNoCancel);
            if (ok != DialogResult.Yes) return;
            var model=EditorModelIO.Load(FullPath);
            model.Montage.Montaged = false;
            EditorModelIO.Save(model);
            Montaged = false;
        }

        public RelayCommand StartEditorCommand { get; private set; }
        public RelayCommand ResetMontageCommand { get; private set; }
    }
} 