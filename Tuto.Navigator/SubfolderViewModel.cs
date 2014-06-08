using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Tuto.Navigator
{
    public class SubfolderViewModel
    {
        // model? viewmodel?

        public SubfolderViewModel(string fullPath)
        {
            FullPath = fullPath;
            StartEditorCommand = new Command(StartEditor);
        }

        public string FullPath { get; private set; }

        public string Name {get { return Path.GetFileName(FullPath); }}

        public void StartEditor()
        {
            // run exe? run from code? block window? need to refresh data?
#if DEBUG
            var editorExe = new FileInfo("c:\\tuto\\tuto.editor\\bin\\debug\\editor.exe");
#endif
            Shell.Exec(false, editorExe, FullPath);
        }

        public Command StartEditorCommand { get; private set; }
    }
}