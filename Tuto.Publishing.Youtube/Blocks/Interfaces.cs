using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tuto.Navigator;

namespace Tuto.Publishing
{
	public interface IMaterialSource
	{
		void Load(IItem root);
		void Pull(IItem root);
		void Save(IItem root);
		ICommandBlockModel ForVideo(VideoWrap wrap);
		ICommandBlockModel ForLecture(LectureWrap wrap);
	}

	public class VisualCommand
	{
		public RelayCommand Command { get; private set; }
		public string ImageSource { get; private set; }
		public VisualCommand(RelayCommand command, string imageSource)
		{
			Command = command;
			ImageSource = imageSource;
		}
	}

	public interface ICommandBlockModel
	{
		List<RelayCommand> Commands { get; }
		string ImageSource { get; }
		Brush Status { get;  }
	}

	public interface ICommandBlocksHolder
	{
		IEnumerable<ICommandBlockModel> CommandBlocks { get; }
	}
}
