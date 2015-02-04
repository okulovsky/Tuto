using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public class ULearnLectureCommands : LectureCommandBlockModel<ULearnSource,ULearnVideoCommands>
	{
		public ULearnLectureCommands(ULearnSource source, LectureWrap wrap)
			: base(source, wrap)
		{
			Commands.Add(new VisualCommand(Compile, () => true, "compile.png"));
            Commands.Add(new VisualCommand(() => Process.Start("\"" + Source.DirectoryForLecture(Wrap).FullName + "\""), () => true, "view.png"));
		}

		public override string ImageFileName
		{
			get { return "ULearn.png"; }
		}


        void Compile()
        {
            foreach (var b in Wrap.ChildCommandBlocks<ULearnVideoCommands>())
                b.Compile();
            File.WriteAllText(
                Path.Combine(Source.DirectoryForLecture(Wrap).FullName, "Title.txt"),
                Wrap.Caption);
        }

		public override BlockStatus Status
		{
			get { return BlockStatus.No(); }
		}
	}
}
