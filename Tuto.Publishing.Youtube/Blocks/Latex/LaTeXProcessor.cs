using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuto.Publishing
{
    class LatexProcessor
    {
        const string Ghostscript = @"C:\Program Files\gs\gs9.15\bin\gswin64.exe";
        const string temporalFileName = "tuto.temp.tex";

        public static void ConvertToPng(FileInfo pdfFile, DirectoryInfo directory)
        {
            pdfFile = pdfFile.CopyTo(Path.Combine(directory.FullName,pdfFile.Name));
            var process=new Process();
            process.StartInfo.FileName=Ghostscript;
            process.StartInfo.Arguments = 
                @"-dBATCH -dNOPAUSE -sDEVICE=pnggray -r300 -dUseCropBox -sOutputFile=""%03d.png"" "+pdfFile.Name;
            process.StartInfo.WorkingDirectory = directory.FullName;
            process.Start();
            process.WaitForExit();
            pdfFile.Delete();
        }

        static bool StartLatex(FileInfo latexFile)
        {
            var process = new Process();
            process.StartInfo.FileName = "pdflatex";
            process.StartInfo.WorkingDirectory = latexFile.Directory.FullName;
            process.StartInfo.Arguments = latexFile.Name;
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        public static FileInfo PrepareSeparateSource(LatexDocument document, DirectoryInfo environmentDirectory)
        {
            var tempLatexFile = new FileInfo(Path.Combine(environmentDirectory.FullName, temporalFileName));
            var builder = new StringBuilder();
            builder.Append(document.Preamble);
            builder.Append("\\begin{document}\r\n");
            foreach (var e in document.Sections)
            {
                builder.Append(e.Name);
                foreach (var s in e.Slides)
                    builder.Append(s.Content);
            }
            builder.Append("\\end{document}");
            File.WriteAllText(tempLatexFile.FullName, builder.ToString());
            return tempLatexFile;
        }

        public static FileInfo Compile(LatexDocument document, DirectoryInfo environmentDirectory)
        {
            var tempLatexFile = PrepareSeparateSource(document, environmentDirectory);            
            if (!StartLatex(tempLatexFile)) return null;
            //StartLatex(tempLatexFile);
            return new FileInfo(Path.Combine(environmentDirectory.FullName, "temp.pdf"));
        }

        public static LatexDocument Parse(FileInfo file)
        {
            var lines = File.ReadAllLines(file.FullName);
            var document = new LatexDocument();
            bool inPreamble = true;
            foreach (var _e in lines)
            {
                var e = _e + "\r\n";
                if (e.Contains("\\begin{document}"))
                {
                    inPreamble = false;
                    continue;
                }
                if (inPreamble)
                {
                    document.Preamble += e;
                    continue;
                }
                if (e.Contains("\\section"))
                {
                    document.Sections.Add(new LatexSection { Name = e });
                    continue;
                }
                if (e.Contains("\\begin{frame}"))
                {
                    document.LastSection.Slides.Add(new LatexSlide { Content = e });
                    continue;
                }
                if (document.LastSection != null && document.LastSection.LastSlide != null)
                    document.LastSection.LastSlide.Content += e;
            }
			document.ModificationTime = file.LastWriteTime;
			document.OriginalFile = file;
            return document;
        }

		public static IEnumerable<LatexDocument> GetAllPresentations(DirectoryInfo directory)
		{
			foreach(var file in directory.GetFiles("*.tex"))
			{
                if (file.Name == temporalFileName) continue;
				var document = Parse(file);
				var sectionDocuments = document.Sections.Select(section =>
					new LatexDocument
					{
						Preamble = document.Preamble,
						Sections = { section },
						ModificationTime = document.ModificationTime,
						OriginalFile = document.OriginalFile
					}).ToList();
				foreach (var doc in sectionDocuments)
					yield return doc;
			}
		}

    }
}
