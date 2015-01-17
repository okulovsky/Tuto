using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing
{
    class LatexLectureCommands : LectureCommandBlockModel<LatexSource,LatexVideoCommands>
    {
        public LatexLectureCommands(LatexSource source, LectureItem item)
            : base(source, item)
        {
            Commands.Add(new VisualCommand(CompileAll, () => true, "compile.png"));
        }


        void CompileAll()
        {
            foreach (var e in VideoData)
                e.Compile();
        }

        public override string ImageFileName
        {
            get { return "latex.png"; }
        }

        public override System.Windows.Media.Brush Status
        {
            get
            {
                if (VideoData.Any(z => z.Status == Brushes.Red)) return Brushes.Red;
                if (VideoData.Any(z=>z.Status==Brushes.Green)) return Brushes.Green;
                return Brushes.Gray;
            }
        }
    }
}
