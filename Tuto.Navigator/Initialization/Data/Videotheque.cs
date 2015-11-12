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
    public class VideothequeLoadingReport
    {
        public string PathName { get; set; }
        public bool OK { get; set; }
    }

    public class VideothequeLoadingRequestItem
    {
        public string Prompt { get; set; }
        public bool IsFolderRequest { get; set; }
        public bool IsFileRequest { get; set; }
        public string SuggestedPath { get; set; }
        public Func<FileInfo> Action { get; set; }
        public VideothequeLoadingRequestItem()
        {
            Action = ()=> { if (SuggestedPath==null) return null; return new FileInfo(SuggestedPath); };
        }
    }

    public class VideothequeLoadingUI
    {
        public Action<VideothequeLoadingReport> Report;
        public Func<VideothequeLoadingRequestItem[], VideothequeLoadingRequestItem> Request;
        public Func<bool> Stop;
    }

	public class Videotheque
	{
        public VideothequeSettings Settings { get; private set; }
        public VideothequeEnvironmentSettings EnvironmentSettings { get; private set; }
        public DirectoryInfo ProgramFolder { get; private set; }
        public VideothequeLocations Locations { get; private set; }



        static void CheckFile(FileInfo path, VideothequeLoadingUI ui, params VideothequeLoadingRequestItem[] requestItems)
        {
            while (true)
            {
                bool ok = File.Exists(path.FullName);
                ui.Report(new VideothequeLoadingReport { PathName = path.FullName, OK = ok });
                if (ok) return;
                if (requestItems.Length==0) break;
                var selectedOption = ui.Request(requestItems);
                if (selectedOption == null) break;
                path = selectedOption.Action();
                if (path == null) break;
            }
            throw new Exception();            
        }

        public static void Load(string videothequeFileName, VideothequeLoadingUI ui)
        {
            var v = new Videotheque();
            v.ProgramFolder = new DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);

            CheckFile(v.Locations.GNP, ui);
            CheckFile(v.Locations.NR, ui);
            CheckFile(v.Locations.PraatExecutable, ui);
            


        }
	}
}
