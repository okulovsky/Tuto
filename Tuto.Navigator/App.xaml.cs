using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Tuto.Model;
using Application = System.Windows.Application;

namespace Tuto.Navigator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void AppStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            var globalModel = new GlobalViewModel();
            mainWindow.DataContext = globalModel;
#if DEBUG
            var dir = EditorModelIO.SubstituteDebugDirectories("work\\");
            var file = Path.Combine(dir, "project.tuto");
            globalModel.Load(new FileInfo(file));
#endif
            mainWindow.Show();
        }
    }
}
