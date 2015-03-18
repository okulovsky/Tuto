using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;
using Tuto;
using Tuto.Publishing.YoutubeData;
using System.IO;

namespace Tuto.Publishing
{
	public class YoutubeVideoCommands : VideoCommandBlockModel<YoutubeSource,YoutubeLectureCommands>
	{
        public override string ImageFileName
        {
            get { return "youtube.png"; }
        }

		override public IEnumerable<BlockStatus> Status
		{
			get
			{
                if (YoutubeClip == null) 
					yield return BlockStatus.Manual("Video is not found on YouTube").PreventExport();
                else if (YoutubeClip.Name != dueTitle || YoutubeClip.Description != dueDescription)
                    yield return BlockStatus.Auto("Video title or description do not match");
                else
					yield return BlockStatus.OK();
			}
		}

        public YoutubeVideoCommands(YoutubeSource source, VideoWrap wrap)
            : base(source,wrap)
		{
         	InitializeDueNames();
			Commands.Add(new VisualCommand(new RelayCommand(CmGo, () => YoutubeClip != null), "view.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmPush, () => YoutubeClip != null), "upload.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmThumbnail, ()=>YoutubeClip!=null), "thumbnail.png"));
		}

		#region Разборка с именами
		string dueTitle;
		string dueDescription;

		void InitializeDueNames()
		{
			var prefix = "";
			var description = "";
            prefix += Source.Settings.CourseAbbreviation;
			var path = Wrap.PathFromRoot.Skip(1).ToArray();
			for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
			{

				TopicLevel level = new TopicLevel();
                if (levelNumber < Source.Settings.TopicLevels.Count)
                    level = Source.Settings.TopicLevels[levelNumber];
				prefix += "-";
				prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic + 1);

				var topic = path[levelNumber] as FolderOrLectureItem;
				if (topic != null)
				{
					description += level.Caption + " " + (path[levelNumber].NumberInTopic + 1) + ". " + topic.Topic.Caption + "\r\n";
				}
			}

			dueTitle = prefix + " " + Wrap.Video.Name;
			description += Source.Settings.DescriptionPS;
			dueDescription = description;
		}
		#endregion

		public YoutubeClip YoutubeClip { get { return Wrap.Get<YoutubeClip>(); } }

		public void CmPush()
		{
			var YoutubeClip = Wrap.Get<YoutubeClip>();
			if (YoutubeClip == null) return;
			var clip = new YoutubeClip { Id = YoutubeClip.Id };
			clip.Name = dueTitle;
			clip.Description = dueDescription;
			clip.UpdateGuid(Wrap.Guid);
			Source.YoutubeProcessor.UpdateVideo(clip);
			Wrap.Store<YoutubeClip>(clip);
            MakeChange();
		}

		public void CmGo()
		{
			if (YoutubeClip == null) return;
			Process.Start(YoutubeClip.VideoURLFull);
		}

		public void CmThumbnail()
		{
			if (YoutubeClip == null) return;
			if (Source.Settings.ThumbnailImagePath == null) return;
			Source.YoutubeProcessor.UpdateVideoThumbnail(YoutubeClip,
				new System.IO.FileInfo(Path.Combine(Source.Settings.Location.FullName,Source.Settings.ThumbnailImagePath)));
		}



		public override void TryMakeItRight()
		{
			if (Status.StartAutoCorrection())
				CmPush();
		}
	}
}
