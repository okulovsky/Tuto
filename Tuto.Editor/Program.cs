using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using Editor.Windows;
using Tuto.Model;


namespace Editor
{
    class Program
    {
        public static string MontageFile="montage.editor";
        public static string TimesFile="times.txt";

        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Pass the argument to the program: the directory with movies");
                return;
            }


            var model = EditorModelIO.Load(EditorModelIO.SubstituteDebugDirectories(args[0]));

            if (model.Montage.SoundIntervals == null || model.Montage.SoundIntervals.Count == 0)
            {
                new Tuto.Services.PraatService().DoWork(model);
            }

             var window = new MainWindow();
            window.DataContext=model;
            new Application().Run(window);
        }
    }
}
