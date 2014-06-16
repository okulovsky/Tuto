using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Tuto.Navigator
{
    public class SubfolderViewModel
    {
        public SubfolderViewModel(string fullPath)
        {
            FullPath = fullPath;
            StartEditorCommand = new RelayCommand(StartEditor);
        }

        public string FullPath { get; private set; }

        public string Name {get { return Path.GetFileName(FullPath); }}

        public void StartEditor()
        {
#if DEBUG
            var editorExe = new FileInfo("c:\\tuto\\tuto.editor\\bin\\debug\\editor.exe");
            Shell.Exec(false, editorExe, FullPath);
#endif
        }

        public RelayCommand StartEditorCommand { get; private set; }
    }
} 