using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public class ULearnSource : IMaterialSource
	{
		public DirectoryInfo ULearnDirectory { get; private set; }
	
		public void Initialize(PublishingSettings settings)
		{
			if (settings.UlearnCourseDirectory != null) 
				ULearnDirectory = new DirectoryInfo(settings.UlearnCourseDirectory);
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