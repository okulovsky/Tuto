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

        public void ConvertToPng(FileInfo pdfFile, DirectoryInfo directory)
        {
            pdfFile = pdfFile.CopyTo(Path.Combine(directory.FullName,pdfFile.Name));
            var process=new Process();
            process.StartInfo.FileName=Program.Ghostscript;
            process.StartInfo.Arguments = 
                @"-dBATCH -dNOPAUSE -sDEVICE=pnggray -r300 -dUseCropBox -sOutputFile=""%03d.png"" "+pdfFile.Name;
            process.StartInfo.WorkingDirectory = directory.FullName;
            process.Start();
            process.WaitForExit();
            pdfFile.Delete();
        }

        static void StartLatex(FileInfo latexFile)
        {
            var process = new Process();
            process.StartInfo.FileName = "pdflatex";
            process.StartInfo.WorkingDirectory = latexFile.Directory.FullName;
            process.StartInfo.Arguments = latexFile.Name;
            process.Start();
            process.WaitForExit();
        }

        public FileInfo Compile(LatexDocument document, DirectoryInfo environmentDirectory)
        {
            var tempLatexFile = new FileInfo(Path.Combine(environmentDirectory.FullName, "temp.tex"));
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
            StartLatex(tempLatexFile);
            //StartLatex(tempLatexFile);
            return new FileInfo(Path.Combine(environmentDirectory.FullName, "temp.pdf"));
        }

        public LatexDocument Parse(FileInfo file)
        {
            var lines = File.ReadAllLines(file.FullName);
            var document = new LatexDocument();
            bool inPreamble = true;
            foreach (var e in lines)
            {
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

		public IEnumerable<LatexDocument> GetAllPresentations(DirectoryInfo directory)
		{
			foreach(var file in directory.GetFiles("*.tex"))
			{
				var document = Parse(file);
				var sectionDocuments = document.Sections.Select(section =>
					new LatexDocument
					{
						Preamble = document.Preamble,
						Sections = { section },
						ModificationTime = document.ModificationTime,
						OriginalFile = document.OriginalFile
					});
				foreach (var doc in sectionDocuments)
					yield return doc;
			}
		}

    }
}
