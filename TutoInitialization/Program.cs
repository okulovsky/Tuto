using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model2;

namespace Tuto.Init
{
	class Program
	{
		[STAThread]
		public static void Main()
		{
			//File.Delete("startup.json");
			var wnd = new MainWindow();
			Func<Videotheque> start = () => Videotheque.Load(null, wnd);
			start.BeginInvoke(null, null);
			new Application().Run(wnd);
		}
	}
}
