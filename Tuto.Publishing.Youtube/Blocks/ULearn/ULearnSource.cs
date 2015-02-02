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
	
		public void Initialize(PublishingSettings settings)
		{
            Settings = settings;
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

        public DirectoryInfo DirectoryForLecture(LectureWrap wrap)
        {
            return new DirectoryInfo(Path.Combine(
                Settings.UlearnCourseDirectory,
                string.Format("L{0:D2} - {1}", wrap.LectureNumber, FileConvert(wrap.Caption))));
        }

        public FileInfo FileForSlide(VideoWrap wrap)
        {
            return new FileInfo(Path.Combine(
                DirectoryForLecture(wrap.Parent as LectureWrap).FullName,
                string.Format("S{0:D2} - {1}.cs", wrap.NumberInTopic, wrap.Caption)));
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