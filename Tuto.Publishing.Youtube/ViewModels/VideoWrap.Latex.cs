using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tuto.Navigator;

namespace Tuto.Publishing
{
    public class VideoWrapLatexModel : NotifierModel
    {
		readonly VideoWrap Wrap;
        public RelayCommand Open { get; private set; }
        public RelayCommand Compile { get; private set; }
        public RelayCommand Edit { get; private set; }

		LatexDocument Source { get { return Wrap.Get<LatexDocument>(); } }

		public Brush Status
		{
			get 
			{ 
				if (Source == null) return Brushes.Gray;
				return Brushes.Red;
			}
		}

        public VideoWrapLatexModel(VideoWrap wrap)
		{
			Wrap = wrap;
			Edit = new RelayCommand(
				()=>{ if (Source!=null) Process.Start("\""+Source.OriginalFile+"\""); },
				()=>Source!=null);
		}
    }
}
