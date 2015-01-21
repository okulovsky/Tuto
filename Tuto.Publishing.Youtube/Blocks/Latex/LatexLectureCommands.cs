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
        public LatexLectureCommands(LatexSource source, LectureWrap item)
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

        public override BlockStatus Status
        {
            get
            {
                if (VideoData.Any(z => z.Status.Status == Statuses.Error)) return BlockStatus.Error("One ore more videos have errors", true);
				if (VideoData.Any(z => z.Status.Status == Statuses.Warning)) return BlockStatus.Warning("One or more video have warninhs", true);
                if (VideoData.Any(z => z.Status.Status == Statuses.OK)) return BlockStatus.OK();
                return BlockStatus.NA();
            }
        }

    }
}
