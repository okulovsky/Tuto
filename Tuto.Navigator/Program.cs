using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Tuto.Model;
using Editor;

namespace Tuto.Navigator
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            TutoProgram.SetSilentMode();
            var mainWindow = new MainNavigatorWindow();
            var globalModel = new GlobalViewModel();
            mainWindow.DataContext = globalModel;
            mainWindow.WindowState = System.Windows.WindowState.Maximized;

			string directoryName = args[0];
			if (File.Exists(args[0]))
			{
				directoryName = new FileInfo(args[0]).Directory.FullName;
			}
			
			var path = EditorModelIO.SubstituteDebugDirectories(directoryName);
            var file = System.IO.Path.Combine(path,"Project.tuto");
            globalModel.Load(new FileInfo(file));

            new Application().Run(mainWindow);
        }

        public static string MontageFile="montage.editor";
        public static string TimesFile="times.txt";

    }
}
