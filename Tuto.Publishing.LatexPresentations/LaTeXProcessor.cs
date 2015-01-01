using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuto.Publishing.LatexPresentations
{
    class LatexProcessor
    {
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
            return document;
        }
    }
}
