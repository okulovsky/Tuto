using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Publishing.YoutubeData;
using YoutubeApiTest;

namespace Tuto.Publishing
{
	public static class StaticItems
	{
		public static readonly IYoutubeProcessor YoutubeProcessor = new YoutubeApisProcessor();
		public static readonly LatexProcessor LatexProcessor = new LatexProcessor();
		public static GlobalData GlobalData { get; set; }
	}
}
