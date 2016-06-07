using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tuto.BatchWorks;
using Tuto.Model;

namespace Tuto.ConsoleMode
{


    public static class TutoProgram
    {

		public static int Assemble(string[] args)
		{
			if (args.Length != 3)
			{
				Console.WriteLine("Arguments missing, required: path to videotheque; guid to assemble; path to desired output");
				return 1;
			}
			var videotheque = Videotheque.Load(args[0], new ConsoleLoadingUI(), false, "..\\..\\..\\Tuto.Navigator\\bin\\Debug");
			Console.WriteLine("Videotheque loaded");
			Console.WriteLine();

			Guid guid = Guid.Empty;
			try
			{
				guid = Guid.Parse(args[1]);
			}
			catch
			{
				Console.WriteLine("GUID '" + args[1] + "' is not a correct guid");
				return 1;
			}
			var model = videotheque.EditorModels.Where(z => z.Montage.Information.Episodes.Any(x => x.Guid == guid)).FirstOrDefault();
			if (model == null)
			{
				Console.WriteLine("GUID '" + args[1] + "' is not found in videotheque");
				return 1;
			}
			var episode = model.Montage.Information.Episodes.Where(z => z.Guid == guid).FirstOrDefault();

			var work = new AssemblyEpisodeWork(model, episode);

			Console.WriteLine(work.Name);
			Console.WriteLine("Press ESC to cancel");
			work.SubsrcibeByExpression(z => z.Progress, () => Console.Write("\r{0:0.00}%        ", work.Progress));


			var queue = new WorkQueue(videotheque.Data.WorkSettings);
			queue.Dispatcher = Dispatcher.CurrentDispatcher;
			queue.Run(new[] { work });
			while (work.Status == BatchWorkStatus.Running || work.Status == BatchWorkStatus.Pending)
			{
				Thread.Sleep(10);
				if (Console.KeyAvailable)
					if (Console.ReadKey(true).Key == ConsoleKey.Escape)
					{
						queue.CancelTask(null);
						Environment.Exit(1);
						return 1;
					}
			}

			if (work.Status == BatchWorkStatus.Success)
			{
				if (File.Exists(args[2])) File.Delete(args[2]);
				File.Move(model.Locations.GetOutputFile(episode).FullName, args[2]);
				Environment.Exit(0);
				return 0;
			}


			var errors = work.WorkTree.Select(z => z.ExceptionMessage).Where(z => !string.IsNullOrEmpty(z)).ToList();
			foreach (var e in errors)
				Console.WriteLine(e);

			Environment.Exit(1);
			return 1;
		}

		public static int Summary(string[] args)
		{
			if (args.Length!=2)
			{
				Console.WriteLine("Arguments missing, required: path to videotheque; output file with summaries");
				return 1;

			}
			var videotheque = Videotheque.Load(args[0], new ConsoleLoadingUI(), false, "..\\..\\..\\Tuto.Navigator\\bin\\Debug");
			Console.WriteLine("Videotheque loaded");
			Console.WriteLine();
			videotheque.CreateSummary(args[1]);
			return 1;
		}

        public static int Main(string[] args)
        {
			if (args.Length==0)
			{
				Console.WriteLine("Specify the mode: assemble or summary");
			}
			if (args[0] == "assemble")
				return Assemble(args.Skip(1).ToArray());
			if (args[0] == "summary")
				return Summary(args.Skip(1).ToArray());


			Console.WriteLine("Undefined mode");
			return 1;
        }
    }
}