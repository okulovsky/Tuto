using System;
using System.Collections.Generic;
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
            Directory = Source.ULearnDirectory.CreateSubdirectory(string.Format("L{0:D2} - {1}", Wrap.NumberInTopic, Source.FileConvert(Wrap.Caption)));
		}

		public override string ImageFileName
		{
			get { return "ULearn.png"; }
		}

        public readonly DirectoryInfo Directory;

        void Compile()
        {
            foreach (var b in Wrap.ChildCommandBlocks<ULearnVideoCommands>())
                b.Compile();
        }

		public override BlockStatus Status
		{
			get { return BlockStatus.No(); }
		}
	}
}
