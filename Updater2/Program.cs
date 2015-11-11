using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto;

namespace Updater2
{
	class Program
	{
		static void Main(string[] args)
		{
			return;
			var path = "D:\\Misc";
			var inputPath = path+"\\Input";
			var dictionary = new Dictionary<DirectoryInfo,string>();
			Library.MakeHashes(
				new System.IO.DirectoryInfo(path),
				"face.mp4",
				"hash.txt",
				true,
				dictionary
				);

		}
	}
}
