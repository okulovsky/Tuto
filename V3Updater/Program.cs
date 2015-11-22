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
			var dir = new DirectoryInfo(path);
			var globalData = CourseTreeData.Load(dir);
			var root = ItemTreeBuilder.Build<FolderWrap, LectureWrap, VideoWrap>(globalData);
			YoutubeDataBinding.LoadYoutubeData(root, dir);
			var Settings= HeadedJsonFormat.Read<PublishingSettings>(dir);
			foreach(var video in root.Subtree())
			{
				var clip = video.Get<YoutubeClip>();
				if (clip == null) continue;
				var episode =
						v.Episodes.Where(z => z.Item2.Guid == video.Guid).FirstOrDefault();
				if (episode!=null)
				{
					episode.Item2.YoutubeId = clip.Id;
					episode.Item1.Save();
				}
			}
			var pubModel = new PublishingModel
			{
				CourseStructure=globalData.Structure,
				 Videos = globalData.Videos,
				  Settings=Settings,
			};
			var pubName=Path.Combine(ModelsFolderName,dir.Name+"."+Names.PublishingModelExtension);
			HeadedJsonFormat.Write(new FileInfo(pubName), pubModel);
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
