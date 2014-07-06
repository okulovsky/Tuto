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
            Shell.Exec(false, editorExe, FullPath);
        }

        public RelayCommand StartEditorCommand { get; private set; }
    }
} 