using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public class ULearnSource : IMaterialSource
	{

        public PublishingSettings Settings { get; private set; }
        public DirectoryInfo ULearnDirectory { get; private set; }
	
		public void Initialize(PublishingSettings settings)
		{
            Settings = settings;
			if (settings.UlearnCourseDirectory != null) 
				ULearnDirectory = new DirectoryInfo(settings.UlearnCourseDirectory);
		}

        Regex csRegex = new Regex(@"[ \.,:-]");
        Regex fileRegex = new Regex(@"[:\?]");


        public string CSConvert(string str)
        {
            return csRegex.Replace(str, "_");
        }

        public string FileConvert(string str)
        {
            return fileRegex.Replace(str, "_");
        }

		public void Load(Item root)
		{
 			
		}

		public void Pull(Item root)
		{

		}

		public void Save(Item root)
		{
 			
		}

		public ICommandBlockModel ForVideo(VideoWrap wrap)
		{
			return new ULearnVideoCommands(this, wrap);
		}

		public ICommandBlockModel ForLecture(LectureWrap wrap)
		{
			return new ULearnLectureCommands(this, wrap);
		}
	}
}