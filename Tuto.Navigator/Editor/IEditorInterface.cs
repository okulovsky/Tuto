using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tuto.Navigator.Editor
{
	public interface IEditorInterface
	{
		void PlayPause(bool pause);
		event KeyEventHandler KeyDown;
		event Action<int, MouseButtonEventArgs> TimelineMouseDown;
		int FaceVideoPosition { get; set; }
		int DesktopVideoPosition { get; set; }
		bool FaceVideoAvailable { get; }
		bool FaceVisible { set; }
		bool DesktopVideoAvailable { get;  }
		bool DesktopVisible { set; }
		void SetRatio(double ratio);
	}
}
