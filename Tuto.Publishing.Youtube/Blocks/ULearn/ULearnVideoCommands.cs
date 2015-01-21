using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public class ULearnVideoCommands : VideoCommandBlockModel<ULearnSource, ULearnVideoCommands>
	{
		public ULearnVideoCommands(ULearnSource source, VideoWrap wrap)
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
