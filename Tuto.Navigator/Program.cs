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
using System.Threading;
using Tuto.BatchWorks;
using Tuto.Publishing.Youtube;
using Tuto.Navigator.ViewModels;
using Tuto.Navigator.Views;

namespace Tuto.Navigator
{
    public static class Program
    {


        [STAThread]
        public static void Main(string[] args)
        {
            //  NewMain(); return;
           

            string fname = null;
            if (args.Length > 0) fname = args[0];
            var application = new Application();
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;                 
            var wnd = new Tuto.Init.MainWindow();
            Func<Videotheque> start = () => Videotheque.Load(fname, wnd, false);
            var token = start.BeginInvoke(null, null);
            wnd.ShowDialog();
            var videotheque = start.EndInvoke(token);
			if (videotheque == null)
			{
				MessageBox.Show("Cannot initialize Tuto");
				return;
			}

            //YoutubeApisProcessor.Initialize(videotheque.TempFolder);

            var mainWindow = new MainNavigatorWindow();
            WorkQueue = new WorkQueue(videotheque.Data.WorkSettings);
            WorkQueue.Dispatcher = mainWindow.Dispatcher;
            var globalModel = new MainModel(videotheque);
            globalModel.VideothequeModel.FillQueue();

            mainWindow.DataContext = globalModel;
            mainWindow.WindowState = System.Windows.WindowState.Maximized;

			string directoryName = args[0];
			if (File.Exists(args[0]))
			{
				directoryName = new FileInfo(args[0]).Directory.FullName;
			}

            application.ShutdownMode = ShutdownMode.OnMainWindowClose;    
            application.Run(mainWindow);
            application.Shutdown();
        }

        public static string MontageFile="montage.editor";
        public static string TimesFile="times.txt";
        public static WorkQueue WorkQueue { get; set; }


        

    }
}
