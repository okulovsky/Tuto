using System;
using System.Collections.Generic;
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
			Commands.Add(new VisualCommand(() => { }, () => true, "compile.png"));
		}

		public override string ImageFileName
		{
			get { return "ULearn.png"; }
		}

		public override BlockStatus Status
		{
			get { return BlockStatus.No(); }
		}
	}
}
