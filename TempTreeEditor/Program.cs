using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.TreeEditor;

namespace TempTreeEditor
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            var dirInfo = new DirectoryInfo(@"..\..\..\..\AIML\Publishing");
            var data = EditorModelIO.ReadGlobalData(dirInfo);
            var model = new PublishViewModel(data.TopicsRoot, data.VideoData);
            var wnd = new PublishPanel();
            wnd.DataContext = model;
            new Application().Run(wnd);
        }
    }
}
