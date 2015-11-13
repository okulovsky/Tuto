using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
	public enum VideothequeLoadingRequestItemType
	{
		NoFile,
		OpenFile,
		SaveFile,
		Directory

	}

	public class VideothequeLoadingRequestItem
	{
		public string Prompt { get; set; }
		public VideothequeLoadingRequestItemType Type { get; set; }
		public string SuggestedPath { get; set; }
		public Func<string,FileInfo> InitFile { get; set; }
		public Func<string, DirectoryInfo> InitFolder { get; set; }
		public string RequestedFileName { get; set; }

		public VideothequeLoadingRequestItem()
		{
			InitFile = s => { if (s == null) return null; return new FileInfo(s); };
			InitFolder = s => { if (s == null) return null; return new DirectoryInfo(s); };
		}

	}
}
