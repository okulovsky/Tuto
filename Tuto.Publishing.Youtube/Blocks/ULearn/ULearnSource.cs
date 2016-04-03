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

        public PublishingModel Model { get; private set; }

        public void Initialize(PublishingModel model)
		{
            Model = model;
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
                Model.Settings.UlearnCourseDirectory,
                string.Format("L{0:D3} - {1}", (wrap.LectureNumber+1)*10, wrap.Caption)));
        }

		public string FilePrefixForSlide(VideoWrap wrap)
		{
			return string.Format("S{0:D3}", (wrap.NumberInTopic + 1) * 10);
		}

        public FileInfo FileForSlide(VideoWrap wrap)
        {
            return new FileInfo(Path.Combine(
                DirectoryForLecture(wrap.Parent as LectureWrap).FullName,
                string.Format("{0} - {1}.cs", FilePrefixForSlide(wrap), wrap.Caption)));
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