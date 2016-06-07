using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
	class Program
	{
		static string GetName(DirectoryInfo parent, DirectoryInfo child)
		{
			var length = parent.FullName.Length;
			return child.FullName.Substring(length, child.FullName.Length - length);
		}

		static void ListDirectories(DirectoryInfo directory, List<DirectoryInfo> data)
		{
			var dirs = directory.GetDirectories();
			if (dirs.Length == 0)
				data.Add(directory);
			else
				foreach(var d in dirs)
					ListDirectories(d,data);
		}

		static List<string> Make(string _directory)
		{
			var directory = new DirectoryInfo(_directory);
			var dirs = new List<DirectoryInfo>();
			ListDirectories(directory, dirs);
			var names = dirs.Select(z=>GetName(directory,z).ToLower()).ToList();
			return names;

		}

		static void Main(string[] args)
		{
			var dirs1 = Make(args[0]);
			var dirs2 = Make(args[1]);

			HashSet<string> d = new HashSet<string>();
			foreach(var e in dirs1)
			{
				d.Add(e);
			}
			foreach (var e in dirs2)
			{
				if (d.Contains(e))
					d.Remove(e);
			}
			foreach(var e in d)
			{
				Console.WriteLine(e);
			}
		}
	}
}
