using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delfu
{
	class DelfuSettings
	{
		public DateTime BreakTime { get; set; }
		public DateTime RestoreTime { get; set; }
		public int Desync { get; set; }
		public int RestoreMargin { get; set; }
		public int BreakMargin { get; set; }
		public int SyncronisationShift { get; set; }
		public string FFMpegPath { get; set; }

		public DelfuSettings()
		{
			BreakMargin = RestoreMargin = 60;
		}
	}
}
