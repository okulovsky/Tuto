using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto;
using Tuto.Model;

namespace V3Updater
{
	class Program
	{
		static string InputFolderName=@"C:\TestMigration\Input";
		static string ModelsFolderName=@"C:\TestMigration\Models";
		static string[] PublishingModels = new[] { @"C:\TestMigration\ObsoletePublishing" };

		static void Recursive(DirectoryInfo dir)
		{
			var files = dir.GetFiles();
			if (!files.Any(z => z.Name == Names.FaceFileName))
			{
				foreach (var d in dir.GetDirectories())
					Recursive(d);
				return;
			}
			var tutoFile = files.Where(z => z.Name == "local.tuto").FirstOrDefault();
			if (tutoFile == null)
				throw new Exception();

			var hash = Videotheque.ComputeHash(dir, Names.FaceFileName, Names.HashFileName, true);
			var container = HeadedJsonFormat.Read<FileContainer>(tutoFile, "Tuto local file", 3);
			container.MontageModel.SetHash(hash);
			var relativePath = MyPath.RelativeTo(tutoFile.Directory.FullName, InputFolderName);
			var fname = Path.Combine(ModelsFolderName, MyPath.CreateHierarchicalName(relativePath) + "." + Names.ModelExtension);
			HeadedJsonFormat.Write(new FileInfo(fname), container);
		}

		static void Main(string[] args)
		{
			Recursive(new DirectoryInfo(InputFolderName));
		}
	}
}
