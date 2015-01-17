using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;
using Tuto.Navigator;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
	public class YoutubeVideoCommands : CommandsBlockModel<YoutubeSource,VideoWrap>
	{
        public override string ImageFileName
        {
            get { return "youtube.png"; }
        }

		override public Brush Status
		{
			get
			{
				if (YoutubeClip == null) return Brushes.Red;
				if (YoutubeClip.Name != dueTitle || YoutubeClip.Description != dueDescription)
					return Brushes.Yellow;
				return Brushes.Green;
			}
		}

        public YoutubeVideoCommands(YoutubeSource source, VideoWrap wrap)
            : base(source,wrap)
		{
         	InitializeDueNames();
			Commands.Add(new VisualCommand(new RelayCommand(CmGo, () => YoutubeClip != null), "view.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmPush, () => YoutubeClip != null), "upload.png"));
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
			Source.YoutubeProcessor.UpdateVideo(clip);
			Wrap.Store<YoutubeClip>(clip);
			this.NotifyByExpression(z => z.Status);
		}

		public void CmGo()
		{
			if (YoutubeClip == null) return;
			Process.Start(YoutubeClip.VideoURLFull);
		}


	}
}
