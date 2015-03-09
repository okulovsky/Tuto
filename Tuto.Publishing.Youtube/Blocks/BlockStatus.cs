using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing
{

	public enum ErrorLevel
	{
		No,
		Warning,
		AutoCorrection,
		ManualCorrection
	}


	public class BlockStatus
	{
		public ErrorLevel ErrorLevel { get; private set; }
		public string Explanation { get; private set; }

		public bool PreventsExport { get; private set; }
		public Brush Brush { get; private set; }

		public bool InheritedFromChildren { get; private set; }

		public BlockStatus(string message, ErrorLevel level)
		{
			this.Explanation = message;
			this.ErrorLevel = level;
			switch(level)
			{
				case Publishing.ErrorLevel.ManualCorrection: Brush = Brushes.Red; break;
				case Publishing.ErrorLevel.AutoCorrection: Brush = Brushes.Yellow; break;
				case Publishing.ErrorLevel.Warning: Brush = Brushes.Cyan; break;
				case Publishing.ErrorLevel.No: Brush = Brushes.Green; break;
			}
		}

		public BlockStatus WithBrush(Brush brush)
		{
			this.Brush=brush;
			return this;
		}

		public BlockStatus PreventExport()
		{
			this.PreventsExport = true;
			return this;
		}

		public BlockStatus Inherited()
		{
			this.InheritedFromChildren = true;
			return this;
		}

		public static BlockStatus Auto(string message)
		{
			return new BlockStatus(message, ErrorLevel.AutoCorrection);
		}
		public static BlockStatus Manual(string message)
		{
			return new BlockStatus(message, ErrorLevel.ManualCorrection);
		}
		public static BlockStatus Warning(string message)
		{
			return new BlockStatus(message, ErrorLevel.Warning);
		}

		public static BlockStatus OK(string message="")
		{
			return new BlockStatus(message, ErrorLevel.No);
		}
	}

	public static class BlockStatusEnumerableExtensions
	{
		public static bool StartAutoCorrection(this IEnumerable<BlockStatus> status)
		{
			bool yes = false;
			foreach(var e in status.Where(z=>!z.InheritedFromChildren))
			{
				if (e.ErrorLevel == ErrorLevel.ManualCorrection) return false;
				if (e.ErrorLevel == ErrorLevel.AutoCorrection) yes = true;
			}
			return yes;
		}

		public static bool ExportPrevented(this IEnumerable<BlockStatus> status)
		{
			return status.Any(z => z.PreventsExport);
		}

	}
}