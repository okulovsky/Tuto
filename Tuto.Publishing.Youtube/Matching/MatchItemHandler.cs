using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	public class MatchItemHandler<TItem>
	{
		public Func<TItem, string> Caption { get; set; }
		public Action<TItem> Open { get; set; }
	}

}
