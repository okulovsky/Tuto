using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Tuto.Model;
using System.Linq;

namespace Tuto.Navigator
{
    public enum VideoStatus
    {
        Raw,
        Marked,
        Montaged
    }

    public class SubfolderViewModel
    {
        public SubfolderViewModel(EditorModel model)
        {
            
            StartEditorCommand = new RelayCommand(StartEditor);
            FullPath = model.Locations.LocalFilePath.Directory.FullName;
            if (model.Montage.Chunks != null && model.Montage.Chunks.Count > 3)
                Status = VideoStatus.Marked;

            if (model.Montage.Information != null && model.Montage.Information.Episodes.Count>0)
            {
                TotalDuration = model.Montage.Information.Episodes.Sum(z => z.Duration.TotalMinutes);
                EpisodesNames = model.Montage.Information.Episodes
                    .Select(z=>z.Name)
                    .Aggregate((a, b) => a + "\r\n" + b);
            }
            if (model.Montage.Montaged)
                Status = VideoStatus.Montaged;
        }

        public bool Selected { get; set; }

        public VideoStatus Status { get; private set; }

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
    }
} 