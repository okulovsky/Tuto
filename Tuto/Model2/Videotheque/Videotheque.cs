using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Model
{

	public class LoadingException : Exception { }


	public class Videotheque
	{
		public VideothequeSettings Settings { get; private set; }
		public VideothequeEnvironmentSettings EnvironmentSettings { get; private set; }
		public DirectoryInfo ProgramFolder { get; private set; }
		public DirectoryInfo RawFolder { get; private set; }
		public DirectoryInfo ModelsFolder { get; private set; }
		public DirectoryInfo OutputFolder { get; private set; }
		public DirectoryInfo TempFolder { get; private set; }
		public FileInfo VideothequeSettingsFile { get; private set;  }
		public VideothequeLocations Locations { get; private set; }


		const string DefaultRawFolder = "Raw";
		const string DefaultModelFolder = "Models";
		const string DefaultOutputFolder = "Output";
		const string DefaultTempFolder = "Temp";

		private Videotheque()
		{

		}




		public static Videotheque Load(string videothequeFileName, IVideuthequeLoadingUI ui)
		{
			
			Videotheque v = new Videotheque();
			v.Locations = new VideothequeLocations(v);
			try
			{
				v.LoadBuiltInSoftware(ui);
				v.LoadExternalReferences(ui);
				v.LoadVideotheque(videothequeFileName, ui);
				v.CheckSubdirectories(ui);
				ui.ExitSuccessfully();
				return v;
			}
			catch (LoadingException)
			{
				return null;
			}
		}

		#region Checking procedures 
		static T Check<T>(
			T path, 
			IVideuthequeLoadingUI ui,
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

		static FileInfo CheckFile(FileInfo file, IVideuthequeLoadingUI ui, string prompt, params VideothequeLoadingRequestItem[] requestItems)
		{
			return Check(file, ui, f => f.FullName, f => f!=null && File.Exists(f.FullName),
				() =>
				{
					var option = ui.Request(prompt, requestItems);
					if (option == null) return null;
					return option.InitFile(option.SuggestedPath);
				});
		}
		
		DirectoryInfo CheckFolder(DirectoryInfo dir, IVideuthequeLoadingUI ui, string prompt, params VideothequeLoadingRequestItem[] requestItems)
		{
			return Check(dir, ui, d => d.FullName, d => d!=null && Directory.Exists(d.FullName),
				() =>
				{
					var option = ui.Request(prompt, requestItems);
					if (option == null) return null;
					return option.InitFolder(option.SuggestedPath);
				});
		}


		DirectoryInfo CheckVideothequeSubdirectory(string relativeLocation, string defaultName, IVideuthequeLoadingUI ui, string prompt)
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
		void LoadBuiltInSoftware(IVideuthequeLoadingUI ui)
        {
			ProgramFolder = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;

			//initialize built-in components
			CheckFile(Locations.GNP, ui,  "GNP is not found in program's folder. Please reinstall Tuto");
            CheckFile(Locations.NR, ui, "NR is not found in program's folder. Please reinstall Tuto");
			CheckFile(Locations.PraatExecutable, ui,  "PRAAT is not found in program's folder. Please reinstall Tuto");
		}

		static FileInfo CreateDefaultStartup(string location)
		{
			var data = new VideothequeEnvironmentSettings();
			var finfo = new FileInfo(location);
			HeadedJsonFormat.Write(
				finfo,
				data);
			return finfo;
		}
		
		void LoadExternalReferences(IVideuthequeLoadingUI ui)
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

			EnvironmentSettings = HeadedJsonFormat.Read<VideothequeEnvironmentSettings>(Locations.StartupSettings);

			CheckFile(Locations.FFmpegExecutable, ui, 
				"FFMPEG is a free software that processes videofiles. You have to install x64 version of it from http://ffmpeg.org/ prior to using Tuto.",
				new VideothequeLoadingRequestItem
				{
					Prompt="FFMPEG is installed. I'm ready to locate ffmpeg.exe",
					Type =  VideothequeLoadingRequestItemType.OpenFile,
					 RequestedFileName = "ffmpeg.exe",
				}
				);

			CheckFile(Locations.FFmpegExecutable, ui,
				"SOX is a software that processes audiofiles. You have to install it from http://sox.sourceforge.net/ prior to using Tuto.",
				new VideothequeLoadingRequestItem
				{
					Prompt = "SOX is installed. I'm ready to locate sox.exe",
					Type = VideothequeLoadingRequestItemType.OpenFile,
					RequestedFileName = "sox.exe",
				}
				);
		}

		static FileInfo CreateEmptyVideotheque(string location)
		{
			var data = new VideothequeSettings();
			var finfo = new FileInfo(location);
			data.PathsSettings.RawPath = CreateDefaultFolder(finfo, DefaultRawFolder);
			data.PathsSettings.ModelPath = CreateDefaultFolder(finfo, DefaultModelFolder);
			data.PathsSettings.OutputPath = CreateDefaultFolder(finfo, DefaultOutputFolder);
			data.PathsSettings.TempPath = CreateDefaultFolder(finfo, DefaultTempFolder);

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
			var data = new VideothequeSettings();
			data.PathsSettings.TempPath = CreateDefaultFolder(finfo, DefaultTempFolder);

			HeadedJsonFormat.Write(
				finfo,
				data);
			return finfo;
		}

		void LoadVideotheque(string videothequeFileName, IVideuthequeLoadingUI ui)
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

			options.AddRange(EnvironmentSettings.LastLoadedProjects.Take(3).Select(z => new VideothequeLoadingRequestItem
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
			Settings = HeadedJsonFormat.Read<VideothequeSettings>(VideothequeSettingsFile);
		}

		void CheckSubdirectories(IVideuthequeLoadingUI ui)
		{
			//hooray! videotheque is loaded! Checking Input, Output and other directories

			RawFolder = CheckVideothequeSubdirectory(Settings.PathsSettings.RawPath, DefaultRawFolder, ui, "Can't locate the folder with the raw video files (ones you get from camera)");
			ModelsFolder = CheckVideothequeSubdirectory(Settings.PathsSettings.ModelPath, DefaultModelFolder, ui, "Can't locate the folder where the markup (the result of your work) is stored)");
			OutputFolder = CheckVideothequeSubdirectory(Settings.PathsSettings.OutputPath, DefaultOutputFolder, ui,  "Can't locate the folder with the output video will be stored");
			RawFolder = CheckVideothequeSubdirectory(Settings.PathsSettings.TempPath, DefaultTempFolder, ui, "Can't locate the folder with the temporary files");

		}
		#endregion

	}
}