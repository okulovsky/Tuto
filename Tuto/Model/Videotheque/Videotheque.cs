using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Publishing;

namespace Tuto.Model
{

	public class LoadingException : Exception { }


    public class RawFileHashes<T>
    {
        public readonly Dictionary<string, T> Hashes = new Dictionary<string, T>();
        public readonly Dictionary<string, List<T>> DuplicatedHashes = new Dictionary<string, List<T>>();
		public void Add(string hash, T value)
		{
			if (Hashes.ContainsKey(hash))
			{
				if (!DuplicatedHashes.ContainsKey(hash))
					DuplicatedHashes[hash] = new List<T> { Hashes[hash] };
				DuplicatedHashes[hash].Add(value);
			}
			else
				Hashes[hash] = value;
		}
		public string GetMessage(Func<T, string> selector)
		{
			if (DuplicatedHashes.Count==0) return null;
			return DuplicatedHashes
				.Select(v => "Hash " + v.Key + ": " + v.Value.Select(selector).Aggregate((a, b) => a + "," + b))
				.Aggregate((a, b) => a + "\r\n" + b);
		}
    }

	public class Videotheque
	{
		public VideothequeData Data { get; private set; }
		public VideothequeStartupSettings StartupSettings { get; private set; }
		public DirectoryInfo ProgramFolder { get; private set; }
		public DirectoryInfo RawFolder { get; private set; }
		public DirectoryInfo ModelsFolder { get; private set; }
		public DirectoryInfo OutputFolder { get; private set; }
		public DirectoryInfo TempFolder { get; private set; }
		public FileInfo VideothequeSettingsFile { get; private set;  }
		public VideothequeLocations Locations { get; private set; }





        Dictionary<string, DirectoryInfo> binaryHashes;
		List<Tuple<FileContainer,FileInfo>> loadedContainer;
        List<EditorModel> models;
        public IEnumerable<EditorModel> EditorModels { get { return models; } }

		List<PublishingModel> publisingModels;
		public IEnumerable<PublishingModel> PublishingModels { get { return publisingModels; } }
		
		public IEnumerable<Tuple<EditorModel,EpisodInfo>> Episodes
		{
			get
			{
				return EditorModels.SelectMany(z => z.Montage.Information.Episodes.Select(x => Tuple.Create(z, x)));
			}
		}

        public Tuple<EditorModel,EpisodInfo> FindEpisode(Guid guid)
        {
            return Episodes.Where(z => z.Item2.Guid == guid).FirstOrDefault();
        }

        public PublishingModel CreateNewPublishingModel(string name)
        {
            var model = new PublishingModel();
            model.Videotheque = this;
            publisingModels.Add(model);
            model.NonDistributedVideos = nonDistributed;
            model.Location = new FileInfo(Path.Combine(ModelsFolder.FullName, name + "." + Names.PublishingModelExtension));
            return model;
        }

		List<VideoPublishSummary> nonDistributed;

		private Videotheque()
		{

		}


        public void Reload()
        {
			var ui = new LoadingUIDecorator(null);
            LoadBinaryHashes(ui);
            LoadContainers(ui);
            CreateModels(ui);
        }

		public static Videotheque Load(string videothequeFileName, IVideothequeLoadingUI ui, bool ignoreExternalSoftware)
		{
			ui = new LoadingUIDecorator(ui);
			Videotheque v = new Videotheque();
			v.ProgramFolder = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
			v.Locations = new VideothequeLocations(v);
			try
			{
				if (!ignoreExternalSoftware)
				{
					v.LoadBuiltInSoftware(ui);
					v.LoadExternalReferences(ui);
					v.SaveStartupFile();
				}
				else
				{
					v.StartupSettings = new VideothequeStartupSettings();
				}

				v.LoadVideotheque(videothequeFileName, ui);
                var fname = v.VideothequeSettingsFile.FullName;
                if (v.StartupSettings.LastLoadedProjects.Contains(fname))
                    v.StartupSettings.LastLoadedProjects.Remove(fname);
                v.StartupSettings.LastLoadedProjects.Insert(0, fname);
                v.SaveStartupFile();
                
				v.CheckSubdirectories(ui);
                v.Save();


				v.LoadBinaryHashes(ui);
				v.LoadContainers(ui);
                v.CreateModels(ui);

				v.LoadPublishing(ui);

				ui.ExitSuccessfully();
				return v;
			}
			catch (LoadingException)
			{
				return null;
			}
		}

		string RelativeOrAbsoluteDirection(DirectoryInfo path)
		{
			var root = VideothequeSettingsFile.Directory.FullName;
			if (path.FullName.StartsWith(root))
				return MyPath.RelativeTo(path.FullName, root);
			else
				return path.FullName;
		}

        public void Save()
        {
			Data.PathsSettings.RawPath = RelativeOrAbsoluteDirection(RawFolder);
			Data.PathsSettings.OutputPath = RelativeOrAbsoluteDirection(OutputFolder);
			Data.PathsSettings.ModelPath = RelativeOrAbsoluteDirection(ModelsFolder);
			Data.PathsSettings.TempPath = RelativeOrAbsoluteDirection(TempFolder);

            HeadedJsonFormat.Write(VideothequeSettingsFile, Data);
        }

        void SaveStartupFile()
        {
            HeadedJsonFormat.Write(Locations.StartupSettings, StartupSettings);
        }

        public void SaveEditorModel(EditorModel model)
        {
            var container = new FileContainer { MontageModel = model.Montage, WindowState = model.WindowState };
            HeadedJsonFormat.Write(model.ModelFileLocation, container);
        }

		#region Checking procedures 
		static T Check<T>(
			T path, 
			IVideothequeLoadingUI ui,
			Func<T,string> name, 
			Func<T,bool> isOk,
			Func<T> requestNew
			)
			where T : class
		{
			while (true)
			{
				if (path != null)
				{
					ui.StartPOSTWork("Checking " + name(path));
					bool ok = isOk(path);
					ui.CompletePOSTWork(ok);
					if (ok) return path;
				}
				path = requestNew();
				if (path == null) break;
			}

			throw new LoadingException();
		}

		static FileInfo CheckFile(FileInfo file, IVideothequeLoadingUI ui, string prompt, params VideothequeLoadingRequestItem[] requestItems)
		{
			return Check(file, ui, f => f.FullName, f => f!=null && File.Exists(f.FullName),
				() =>
				{
					var option = ui.Request(prompt, requestItems);
					if (option == null) return null;
					return option.InitFile(option.SuggestedPath);
				});
		}
		
		DirectoryInfo CheckFolder(DirectoryInfo dir, IVideothequeLoadingUI ui, string prompt, params VideothequeLoadingRequestItem[] requestItems)
		{
			return Check(dir, ui, d => d.FullName, d => d!=null && Directory.Exists(d.FullName),
				() =>
				{
					var option = ui.Request(prompt, requestItems);
					if (option == null) return null;
					return option.InitFolder(option.SuggestedPath);
				});
		}


		DirectoryInfo CheckVideothequeSubdirectory(string relativeLocation, string defaultName, IVideothequeLoadingUI ui, string prompt)
		{
			DirectoryInfo dir = null;
			if (relativeLocation != null)
			{
				if (Path.IsPathRooted(relativeLocation)) dir =  new DirectoryInfo(relativeLocation);
				dir = new DirectoryInfo(Path.Combine(VideothequeSettingsFile.Directory.FullName, relativeLocation));
			}
			return CheckFolder(dir, ui, prompt,
				new VideothequeLoadingRequestItem
				{
					Prompt = "Locate the folder",
					Type = VideothequeLoadingRequestItemType.Directory
				},
				new VideothequeLoadingRequestItem
				{
					Prompt = "Create an empty folder at the default location",
					Type = VideothequeLoadingRequestItemType.NoFile,
					SuggestedPath = Path.Combine(VideothequeSettingsFile.Directory.FullName, defaultName),
					InitFolder = s => { if (s == null) return null; Directory.CreateDirectory(s); return new DirectoryInfo(s); }
				}

				);
		}
		#endregion

		#region Basic loading procedures
		void LoadBuiltInSoftware(IVideothequeLoadingUI ui)
        {
		
			//initialize built-in components
			CheckFile(Locations.GNP, ui,  "GNP is not found in program's folder. Please reinstall Tuto");
            CheckFile(Locations.NR, ui, "NR is not found in program's folder. Please reinstall Tuto");
			CheckFile(Locations.PraatExecutable, ui,  "PRAAT is not found in program's folder. Please reinstall Tuto");
		}

		static FileInfo CreateDefaultStartup(string location)
		{
			var data = new VideothequeStartupSettings();
			var finfo = new FileInfo(location);
			HeadedJsonFormat.Write(
				finfo,
				data);
			return finfo;
		}

        void LoadExternalReferences(IVideothequeLoadingUI ui)
        {
            //loading startup settings
            CheckFile(Locations.StartupSettings, ui,
                "Startup settings are not found. It is probably the first time you start Tuto. ",
                new VideothequeLoadingRequestItem
                    {
                        Prompt = "Create default startup settings. Be ready to locate external software",
                        SuggestedPath = Locations.StartupSettings.FullName,
                        Type = VideothequeLoadingRequestItemType.NoFile,
                        InitFile = CreateDefaultStartup
                    });

            StartupSettings = HeadedJsonFormat.Read<VideothequeStartupSettings>(Locations.StartupSettings);
            FileInfo file;

            file = CheckFile(Locations.FFmpegExecutable, ui,
                "FFMPEG is a free software that processes videofiles. You have to install x64 version of it from http://ffmpeg.zeranoe.com/builds/ prior to using Tuto.",
                new VideothequeLoadingRequestItem
                {
                    Prompt = "FFMPEG is installed. I'm ready to locate ffmpeg.exe",
                    Type = VideothequeLoadingRequestItemType.OpenFile,
                    RequestedFileName = "ffmpeg.exe",
                }
                );
            StartupSettings.FFMPEGPath = file.FullName;

            file = CheckFile(Locations.SoxExecutable, ui,
                "SOX is a free software that processes audiofiles. You have to install it from http://sox.sourceforge.net/ prior to using Tuto.",
                new VideothequeLoadingRequestItem
                {
                    Prompt = "SOX is installed. I'm ready to locate sox.exe",
                    Type = VideothequeLoadingRequestItemType.OpenFile,
                    RequestedFileName = "sox.exe",
                }
                );
            StartupSettings.SoxPath = file.FullName;

            var directory = CheckFolder(Locations.AviSynth, ui,
                "AviSynth is a free software that enables scripting of videofiles. You have to install it from http://avisynth.nl/index.php/AviSynth+#Development_branch , version greater than r1800, prior to using Tuto.",
                new VideothequeLoadingRequestItem
                {
                    Prompt = "AviSynth is installed. I'm ready to locate its folder",
                    Type = VideothequeLoadingRequestItemType.Directory,
                }
                );
            StartupSettings.AviSynthPath = directory.FullName;


          
        }

		static FileInfo CreateEmptyVideotheque(string location)
		{
			var data = new VideothequeData();
			var finfo = new FileInfo(location);
            data.PathsSettings.RawPath = CreateDefaultFolder(finfo, Names.DefaultRawFolder);
            data.PathsSettings.ModelPath = CreateDefaultFolder(finfo, Names.DefaultModelFolder);
            data.PathsSettings.OutputPath = CreateDefaultFolder(finfo, Names.DefaultOutputFolder);
            data.PathsSettings.TempPath = CreateDefaultFolder(finfo, Names.DefaultTempFolder);

			HeadedJsonFormat.Write(
				finfo,
				data);
			return finfo;
		}

		static string CreateDefaultFolder(FileInfo vlocation, string dname)
		{
			vlocation.Directory.CreateSubdirectory(dname);
			return dname;
		}

		static FileInfo CreateVideothequeForSetup(string location)
		{
			var finfo = new FileInfo(location);
			var data = new VideothequeData();
            data.PathsSettings.TempPath = CreateDefaultFolder(finfo, Names.DefaultTempFolder);

			HeadedJsonFormat.Write(
				finfo,
				data);
			return finfo;
		}

		void LoadVideotheque(string videothequeFileName, IVideothequeLoadingUI ui)
		{
			var options = new List<VideothequeLoadingRequestItem>();
			options.Add(new VideothequeLoadingRequestItem
			{
				Prompt = "Create new videotheque, and keep all files inside its folder",
				Type = VideothequeLoadingRequestItemType.SaveFile,
			    InitFile = CreateEmptyVideotheque 
			});
			options.Add(new VideothequeLoadingRequestItem
			{
				Prompt = "Create new videotheque, and use it with external folders with data (data will be located later)",
				Type = VideothequeLoadingRequestItemType.SaveFile,
				InitFile = CreateVideothequeForSetup
			});
			options.Add(new VideothequeLoadingRequestItem
				{
					Prompt = "Load existing videotheque",
					Type = VideothequeLoadingRequestItemType.OpenFile,
				});

			options.AddRange(StartupSettings.LastLoadedProjects.Where(z=>File.Exists(z)).Take(3).Select(z => new VideothequeLoadingRequestItem
			{
				Prompt = "Load videotheque " + z,
				Type = VideothequeLoadingRequestItemType.NoFile,
				SuggestedPath = z
			}));

			FileInfo vfinfo = null;
			if (videothequeFileName != null) vfinfo = new FileInfo(videothequeFileName);
			vfinfo = CheckFile(vfinfo, ui,
				"Don't see videotheque",
				options.ToArray()
				);

			VideothequeSettingsFile = vfinfo;
			Data = HeadedJsonFormat.Read<VideothequeData>(VideothequeSettingsFile);
            if (Data.EditorSettings == null) Data.EditorSettings = new VideothequeEditorSettings();
		}

		void CheckSubdirectories(IVideothequeLoadingUI ui)
		{
			//hooray! videotheque is loaded! Checking Input, Output and other directories

            RawFolder = CheckVideothequeSubdirectory(Data.PathsSettings.RawPath, Names.DefaultRawFolder, ui, "Can't locate the folder with the raw video files (ones you get from camera)");
            ModelsFolder = CheckVideothequeSubdirectory(Data.PathsSettings.ModelPath, Names.DefaultModelFolder, ui, "Can't locate the folder where the markup (the result of your work) is stored)");
            OutputFolder = CheckVideothequeSubdirectory(Data.PathsSettings.OutputPath, Names.DefaultOutputFolder, ui, "Can't locate the folder with the output video will be stored");
            TempFolder = CheckVideothequeSubdirectory(Data.PathsSettings.TempPath, Names.DefaultTempFolder, ui, "Can't locate the folder with the temporary files");
		}
		#endregion
		#region Loading files

		public static string ComputeHash(FileInfo file)
		{
			using (var md5 = MD5.Create())
			using (var stream = new BinaryReader(File.OpenRead(file.FullName)))
			{
				var bytes = stream.ReadBytes(10000);
				return BitConverter.ToString(md5.ComputeHash(bytes));
			}
		}

		public static string ComputeHash(DirectoryInfo directory, string targetFileName, string hashFileName, bool recomputeAll)
		{
			string hash = null;
			string localHashFileName = Path.Combine(directory.FullName, hashFileName);
			if (!recomputeAll && File.Exists(localHashFileName))
				hash = File.ReadAllText(localHashFileName);
			else
			{
				hash = ComputeHash(new FileInfo(Path.Combine(directory.FullName, targetFileName)));
				File.WriteAllText(localHashFileName, hash);
			}
			return hash;
		}

		public static void ComputeHashesInRawSubdirectories(DirectoryInfo directory, string targetFileName, string hashFileName, bool recomputeAll, RawFileHashes<DirectoryInfo> hashes)
		{
			var files = directory.GetFiles();
			if (files.Any(z => z.Name == targetFileName))
			{
				string hash = ComputeHash(directory, targetFileName, hashFileName, recomputeAll);
				hashes.Add(hash, directory);
			}
			else foreach (var d in directory.GetDirectories())
					ComputeHashesInRawSubdirectories(d, targetFileName, hashFileName, recomputeAll, hashes);
		}



		void LoadBinaryHashes(IVideothequeLoadingUI ui)
		{
            ui.StartPOSTWork("Indexing videofiles in " + RawFolder.FullName);
            var hashes = new RawFileHashes<DirectoryInfo>();
            ComputeHashesInRawSubdirectories(RawFolder, Names.FaceFileName, Names.HashFileName, false, hashes);
            if (hashes.DuplicatedHashes.Count != 0)
            {
                var message="Some input video files are duplicated. This is not acceptable. Please resolve the issue. The duplications are:\r\n";
                foreach(var e in hashes.DuplicatedHashes)
                    message+=e.Value.Select(z=>z.FullName).Select(z=>MyPath.RelativeTo(z,RawFolder.FullName)).Aggregate((a,b)=>a+", "+b)+"\r\n";
				ui.Request(message, new VideothequeLoadingRequestItem[0]);
                throw new LoadingException();
            }
            binaryHashes = hashes.Hashes;
            ui.CompletePOSTWork(true);
		}

		static void LoadFiles<T>(DirectoryInfo dir,  string extension, List<Tuple<T,FileInfo>> list)
			where T : new()
		{
			foreach (var e in dir.GetFiles("*." + extension))
            {
                var container = HeadedJsonFormat.Read<T>(e);
                list.Add(Tuple.Create(container, e));
            }
				
			foreach (var e in dir.GetDirectories())
				LoadFiles(e, extension, list);
		}

      

		void LoadContainers(IVideothequeLoadingUI ui)
		{
            ui.StartPOSTWork("Loading models");
            loadedContainer = new List<Tuple<FileContainer, FileInfo>>();
			LoadFiles<FileContainer>(ModelsFolder, Names.ModelExtension, loadedContainer);

            ui.CompletePOSTWork(true);
		}


        EditorModel LoadExistingModel(FileContainer container, FileInfo file, DirectoryInfo rawLocation)
        {
            if (rawLocation!=null)
            {
                container.MontageModel.DisplayedRawLocation = MyPath.RelativeTo(rawLocation.FullName, RawFolder.FullName);
            }
            else
            {
               container.MontageModel.DisplayedRawLocation = file.Name;
               rawLocation = new DirectoryInfo("Z:\\");
            }
            var model = new EditorModel(file, rawLocation, this, container.MontageModel, container.WindowState);
            SaveEditorModel(model);
            return model;
        }

        EditorModel CreateNewModel(DirectoryInfo location, string hash)
        {
            var path = MyPath.RelativeTo(location.FullName, RawFolder.FullName);
            path = MyPath.CreateHierarchicalName(path);
            var finfo = new FileInfo(Path.Combine(ModelsFolder.FullName, path + "." + Names.ModelExtension));
            var model = new EditorModel(finfo, location, this, new MontageModel(3600000, hash), new WindowState());
            model.Montage.DisplayedRawLocation = path;
            model.Montage.Information.CreationTime = DateTime.Now;
            model.Montage.Information.LastModificationTime = DateTime.Now;
            return model;
        }

        void CreateModels(IVideothequeLoadingUI ui)
        {
            ui.StartPOSTWork("Creating models");
            models = new List<EditorModel>();
            foreach(var e in loadedContainer)
            {
                var hash = e.Item1.MontageModel.RawVideoHash; 
                if (hash == null) throw new Exception("No reference to video is specified in the model");
                DirectoryInfo rawDirectory = null;
                if (binaryHashes.ContainsKey(hash))
                    rawDirectory = binaryHashes[hash];
                var model = LoadExistingModel(e.Item1, e.Item2, rawDirectory);
                binaryHashes.Remove(hash);
                models.Add(model);
            }
            foreach(var e in binaryHashes)
            {
                var model = CreateNewModel(e.Value,e.Key);
                models.Add(model);
            }
            ui.CompletePOSTWork(true);
        }





		void LoadPublishing(IVideothequeLoadingUI ui)
		{
			ui.StartPOSTWork("Loading publishing models");
			var models = new List<Tuple<PublishingModel, FileInfo>>();
			LoadFiles<PublishingModel>(ModelsFolder, Names.PublishingModelExtension, models);
			publisingModels = new List<PublishingModel>();
			foreach(var e in models)
			{
				e.Item1.Videotheque = this;
				e.Item1.Location = e.Item2;
				publisingModels.Add(e.Item1);
			}
			ui.CompletePOSTWork(true);

			nonDistributed = new List<VideoPublishSummary>();
			foreach (var e in PublishingModels)
				e.NonDistributedVideos = nonDistributed;
			UpdateNonDistributedVideos();
            
            foreach(var e in PublishingModels.SelectMany(z=>z.YoutubeClipData.Records))
            {
                var ep = FindEpisode(e.Guid);
                if (ep != null && ep.Item2.YoutubeId!=null)
                    e.Data.Id = ep.Item2.YoutubeId;
            }


		}

		public void UpdateNonDistributedVideos()
		{
			nonDistributed.Clear();
			foreach(var m in EditorModels)
				foreach (var e in m.Montage.Information.Episodes)
				{
					if (publisingModels.SelectMany(z => z.Videos).Any(z => z.Guid == e.Guid)) continue;
					nonDistributed.Add(new VideoPublishSummary
					{
						Guid = e.Guid,
						Duration = e.Duration,
						Name = e.Name,
						OrdinalSuffix = m.Montage.DisplayedRawLocation + "-"
								+ m.Montage.Information.Episodes.IndexOf(e)
					});
				}
		}
		#endregion
	}
}