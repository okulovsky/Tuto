using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto;
using Tuto.Model;
using Tuto.Publishing;

namespace V3Updater
{
	class Program
	{



		static void Recursive(DirectoryInfo currentDirectory, DirectoryInfo inputRoot, DirectoryInfo newModelsDirectory, string prefix)
		{
			Console.WriteLine("Processing " + currentDirectory.FullName);
			var files = currentDirectory.GetFiles();
			if (!files.Any(z => z.Name == Names.FaceFileName))
			{
				foreach (var d in currentDirectory.GetDirectories())
					Recursive(d, inputRoot, newModelsDirectory, prefix);
				return;
			}
			var tutoFile = files.Where(z => z.Name == "local.tuto").FirstOrDefault();
			if (tutoFile == null)
				throw new Exception();

			var hash = Videotheque.ComputeHash(currentDirectory, Names.FaceFileName, Names.HashFileName, true);
			var container = HeadedJsonFormat.Read<FileContainer>(tutoFile, "Tuto local file", 3);
			container.MontageModel.SetHash(hash);
			var relativePath = prefix+MyPath.RelativeTo(tutoFile.Directory.FullName, inputRoot.FullName);
			container.MontageModel.DisplayedRawLocation = relativePath;
			var fname = Path.Combine(newModelsDirectory.FullName, MyPath.CreateHierarchicalName(relativePath) + "." + Names.ModelExtension);
			HeadedJsonFormat.Write(new FileInfo(fname), container);
		}

		static void UpdateOldLocalTutoFiles(string fromWhere, string newPlace, string prefix)
		{
			Recursive(new DirectoryInfo(fromWhere), new DirectoryInfo(fromWhere), new DirectoryInfo(newPlace), prefix);
		}

		static IEnumerable<IMaterialSource> SourcesFactory()
		{
			yield break;
		}

		//static void ParsePublishing(string path, Videotheque v)
		//{
		//	var pubModel = new PublishingModel();
		//	pubModel.Videotheque=v;
		//	var dir = new DirectoryInfo(path);
		//	var globalData = CourseTreeData.Load(dir);
		//	pubModel.CourseStructure=globalData.Structure;
		//	pubModel.Videos = globalData.Videos;
		//	pubModel.Settings=HeadedJsonFormat.Read<PublishingSettings>(dir);
		//	pubModel.YoutubeClipData=HeadedJsonFormat.Read<DataLayer<YoutubeClip>>(dir);
		//	pubModel.YoutubePlaylistData=HeadedJsonFormat.Read<DataLayer<YoutubePlaylist>>(dir);
		//	var pubName=Path.Combine(ModelsFolderName,dir.Name+"."+Names.PublishingModelExtension);
		//	pubModel.Location = new FileInfo(pubName);
		//	pubModel.Save();
		//}


		static void Main(string[] args)
		{
			UpdateOldLocalTutoFiles(
				@"D:\HACKERDOM\Input",
				@"D:\Montage\Models\OldHackerdomLectures",
				"Hackerdom\\");
		}
	}
}
