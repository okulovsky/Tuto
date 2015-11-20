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
        public static void NewMain()
        {
            var videotheque = Videotheque.Load(@"C:\Users\Yura\Desktop\TestMontage\v", null, true);
            var model = new Tuto.Navigator.NewLook.MainViewModel(videotheque);
            var wnd = new Tuto.Navigator.NewLook.MainWindow();
            wnd.DataContext=model;
            new Application().Run(wnd);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            NewMain(); return;


            var application = new Application();
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;                 
            var wnd = new Tuto.Init.MainWindow();
            Func<Videotheque> start = () => Videotheque.Load(null, wnd, false);
            var token = start.BeginInvoke(null, null);
            wnd.ShowDialog();
            var videotheque = start.EndInvoke(token);
			if (videotheque == null)
			{
				MessageBox.Show("Cannot initialize Tuto");
				return;
			}



            var mainWindow = new MainNavigatorWindow();
            var globalModel = new VideothequeModel(videotheque);
            BatchWorkQueueWindow = globalModel.queueWindow;
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
        public static BatchWorkWindow BatchWorkQueueWindow {get; set;}


        

    }
}
