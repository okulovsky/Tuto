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
        static string FolderName = @"C:\Users\Yura\Desktop\TestMontage\";
		static string InputFolderName=FolderName+"Input";
		static string ModelsFolderName=FolderName+"Models";
		static string[] PublishingModels = new string[] { };
		static string VideothequeFileName = FolderName+"v";


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

		static IEnumerable<IMaterialSource> SourcesFactory()
		{
			yield break;
		}

		static void ParsePublishing(string path, Videotheque v)
		{
            var pubModel = new PublishingModel();
			pubModel.Videotheque=v;
            var dir = new DirectoryInfo(path);
			var globalData = CourseTreeData.Load(dir);
            pubModel.CourseStructure=globalData.Structure;
            pubModel.Videos = globalData.Videos;
            pubModel.Settings=HeadedJsonFormat.Read<PublishingSettings>(dir);
            pubModel.YoutubeClipData=HeadedJsonFormat.Read<DataLayer<YoutubeClip>>(dir);
            pubModel.YoutubePlaylistData=HeadedJsonFormat.Read<DataLayer<YoutubePlaylist>>(dir);
            var pubName=Path.Combine(ModelsFolderName,dir.Name+"."+Names.PublishingModelExtension);
            pubModel.Location = new FileInfo(pubName);
            pubModel.Save();
		}


		static void Main(string[] args)
		{
			Recursive(new DirectoryInfo(InputFolderName));
			var v = Videotheque.Load(VideothequeFileName, null, true);
			foreach (var e in PublishingModels)
				ParsePublishing(e, v);
		}
	}
}
