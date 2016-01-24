using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tuto.Navigator.Editor
{
    public enum VideoType
    {
        Face,
        Desktop,
    }

	public interface IEditorInterface
	{
		event KeyEventHandler KeyDown;
		event Action<int, MouseButtonEventArgs> TimelineMouseDown;
        void SetRatio(double ratio);
        IVideoInterface Face { get; }
        IVideoInterface Desktop { get; }
	}

    public interface IVideoInterface
    {
        int Position { get; set; }
        void SetFile(FileInfo location);
        bool Visibility { get; set; }
        void PlayPause(bool pause);
    }
}
