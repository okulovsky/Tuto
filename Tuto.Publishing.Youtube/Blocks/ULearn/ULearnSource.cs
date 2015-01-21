using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	class ULearnSource : IMaterialSource
	{
		public DirectoryInfo ULearnDirectory { get; private set; }
	
		public void Initialize(PublishingSettings settings)
		{
			ULearnDirectory = new DirectoryInfo(settings.UlearnCourseDirectory);
		}

		public void Load(Item root)
		{
 			
		}

		public void Pull(Item root)
		{
			Load(root);
		}

		public void Save(Item root)
		{
 			
		}

		public ICommandBlockModel ForVideo(VideoWrap wrap)
		{
 			throw new NotImplementedException();
		}

		public ICommandBlockModel ForLecture(LectureWrap wrap)
		{
 			throw new NotImplementedException();
		}
	}
}