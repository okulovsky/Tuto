using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tuto.Model;

namespace Tuto.Navigator.Editor
{
    public enum VideoType
    {
        Face,
        Desktop,
    }
    
   

	public interface IEditorInterface
	{
		event KeyEventHandler ControlKeyDown;
		event Action<int, bool> TimeSelected;
        void SetRatio(double ratio);
        IVideoInterface Face { get; }
        IVideoInterface Desktop { get; }
        IVideoInterface Patch { get;  }
        void SetArrangeMode(ArrangeModes mode);

        void SetSubtitles(SubtitlePatch patch);

        void SetVideoPatch(VideoPatch patch);

        void Refresh();

	}

    public interface IVideoInterface
    {
        int Position { get; set; }
        void SetFile(FileInfo location);
        bool Visibility { get; set; }

        bool Paused { get; set; }

        int GetDuration();

        void Die();
        bool Loaded { get; }

        bool Muted { set; }
    }
}
