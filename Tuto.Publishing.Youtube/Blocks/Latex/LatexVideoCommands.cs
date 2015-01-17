using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing
{
    class LatexVideoCommands : ICommandBlockModel
    {
        VideoWrap wrap;
        public LatexVideoCommands(VideoWrap wrap)
        {
            this.wrap = wrap;
            Commands = new List<VisualCommand>();
        }

        public List<VisualCommand> Commands
        {
            get;
            private set; 
        }

        public Uri ImageSource
        {
            get { return new Uri("/Img/Latex.png", UriKind.Relative); } 
        }

        public System.Windows.Media.Brush Status
        {
            get
            {
                if (wrap.Get<LatexDocument>() == null) return Brushes.Gray;
                return Brushes.Red;
            }
        }
    }
}
