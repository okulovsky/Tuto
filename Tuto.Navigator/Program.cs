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

namespace Tuto.Navigator
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var application = new Application();
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;                 
            var wnd = new Tuto.Init.MainWindow();
            Func<Videotheque> start = () => Videotheque.Load(null, wnd);
            var token = start.BeginInvoke(null, null);
            wnd.ShowDialog();
            var videotheque = start.EndInvoke(token);

            var mainWindow = new MainNavigatorWindow();
            var globalModel = new VideothequeModel(videotheque);

            WorkQueue = new WorkQueue(globalModel);
            var queueWindow = new BatchWorkWindow();
            queueWindow.AssignCancelOperation(WorkQueue.CancelTask);
            queueWindow.DataContext = WorkQueue.Work;
            WorkQueue.Dispatcher = queueWindow.Dispatcher;
            queueWindow.Show();

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
