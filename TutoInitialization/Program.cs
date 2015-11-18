using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;

namespace Tuto.NewMainWindow
{
	class Program
	{
		[STAThread]
		public static void Main()
		{
			//File.Delete("startup.json");
            var model = new List<TestModel> { new TestModel { Name = "abcd" }, new TestModel { Name = "xyz" } };
			var wnd = new MainWindow();
            wnd.DataContext = model;
			new Application().Run(wnd);
		}
	}
}
