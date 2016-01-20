using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delfu
{
	class Program
	{
		static DirectoryInfo directory;
		static DelfuSettings settings;

		static void FFMPEG(string argsFormat, params object[] obs)
		{
			var args = string.Format(argsFormat, obs);
			Console.WriteLine(args);
			File.AppendAllText(Local("log.txt"), settings.FFMpegPath+" "+args+"\r\n");
			var process = new Process();
			process.StartInfo.FileName = settings.FFMpegPath;
			process.StartInfo.Arguments = args;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.WaitForExit();
		}

		static string LocalQ(string fname)
		{
			return "\"" + Path.Combine(directory.FullName, fname) + "\"";
		}

		static string Local(string fname)
		{
			return Path.Combine(directory.FullName, fname);

		}
		static string Time(DateTime time)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", time.Hour, time.Minute, time.Second, time.Millisecond);
		}

		static void TestRestored(bool preview)
		{
			var duration = "";
			if (preview)
				duration = "-t " + settings.RestoreMargin;

			FFMPEG("-i {0} -ss {1} {2} -acodec libmp3lame -y {3}",
				LocalQ("face.mp4"),
				Time(settings.RestoreTime+TimeSpan.FromMilliseconds(settings.Desync)),
				duration,
				LocalQ("part2_audio.mp3"));

			FFMPEG("-i {0} -ss {1} {2} -vcodec copy -y -an {3}",
				LocalQ("face.mp4"),
				Time(settings.RestoreTime),
				duration,
				LocalQ("part2_video.mp4"));

			FFMPEG("-i {0} -i {1} -y {2}",
				LocalQ("part2_video.mp4"),
				LocalQ("part2_audio.mp3"),
				LocalQ("part2.mp4"));

			File.Delete(Local("part2_video.mp4"));
			File.Delete(Local("part2_audio.mp3"));

		}

		static void Sew(string file1, string file2, string result, bool copy)
		{
			File.WriteAllLines(
				Local("list.txt"),
				new [] {
					"file '"+Local(file1)+"'",
					"file '"+Local(file2)+"'"
				});

			var cp = "-qscale 0";
			if (copy)
				cp = "-vcodec copy";

			FFMPEG("-f concat -i {0} {1} -y  {2}",
				LocalQ("list.txt"),
				cp,
				LocalQ(result));

		//	File.Delete(Local("list.txt"));

		}

		static void TestBorder()
		{

				TestRestored(true);
				//C:\ffmpeg\bin\ffmpeg.exe -i %_file% -ss %_before% -t %_beforeduration% -y -vcodec copy part1.mp4
				FFMPEG("-i {0} -ss {1} -t {2} -y {3}",
					LocalQ("face.mp4"),
					Time(settings.BreakTime - TimeSpan.FromSeconds(settings.BreakMargin)),
					settings.BreakMargin,
					LocalQ("part1.mp4"));

			Sew("part1.mp4", "part2.mp4", "face-corrected-sample.mp4",false);
		}

		static void RestoreFace()
		{
			TestRestored(false);
		
			FFMPEG("-i {0} -t {1} -y {2}",
				LocalQ("face.mp4"),
				Time(settings.BreakTime),
				LocalQ("part1.mp4"));

			Sew("part1.mp4", "part2.mp4", "face-corrected.mp4",false);
		}

		static void RestoreDesktop()
		{
			FFMPEG("-i {0} -t {1} -vcodec copy -y {2}",
				LocalQ("desktop.avi"),
				Time(settings.BreakTime - TimeSpan.FromMilliseconds(settings.SyncronisationShift)),
				LocalQ("desk-part-1.avi")
				);
			FFMPEG("-i {0} -ss {1} -vcodec copy  -y {2}",
				LocalQ("desktop.avi"),
				Time(settings.RestoreTime - TimeSpan.FromMilliseconds(settings.SyncronisationShift)),
				LocalQ("desk-part-2.avi"));
			Sew("desk-part-1.avi", "desk-part-2.avi", "desktop-corrected.avi",true);
		}


		static void Main(string[] args)
		{
			var fname="delfu.txt";
			directory = new DirectoryInfo(".");
			if (args.Length != 0)
			{
				directory = new DirectoryInfo(args[0]);
				fname = Path.Combine(directory.FullName, fname);
			}

			if (!File.Exists(fname))
			{
				var sets = new DelfuSettings
				{
					FFMpegPath = @"C:\ffmpeg\bin\ffmpeg.exe"
				};
				File.WriteAllText(fname, Newtonsoft.Json.JsonConvert.SerializeObject(sets, Newtonsoft.Json.Formatting.Indented));
				return;
			}

			var text = File.ReadAllText(fname);
			settings = Newtonsoft.Json.JsonConvert.DeserializeObject<DelfuSettings>(text);

			Console.WriteLine("Select mode");
			Console.WriteLine("1 - test restored");
			Console.WriteLine("2 - test border");
			Console.WriteLine("3 - restore face");
			Console.WriteLine("4 - restore desktop");

			var key = Console.ReadKey(true);
			if (key.Key == ConsoleKey.D1)
				TestRestored(true);
			else if (key.Key == ConsoleKey.D2)
				TestBorder();
			else if (key.Key == ConsoleKey.D3)
				RestoreFace();
			else if (key.Key == ConsoleKey.D4)
				RestoreDesktop();
		}
	}
}
