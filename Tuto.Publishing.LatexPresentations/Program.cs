using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuto.Publishing.LatexPresentations
{
    class Program
    {
        public const string Pdflatex = "pdflatex";
        public const string Ghostscript = @"C:\Program Files\gs\gs9.15\bin\gswin64.exe";
        
     
        static void Main(string[] args)
        {
            var latexFile = new FileInfo(@"..\..\..\..\AIML\Latex\ulearn-lecture-01.tex");
            var processor = new LatexProcessor();
            var document = processor.Parse(latexFile);
            var pdf = processor.Compile(document, latexFile.Directory);
            var result = processor.ConvertToPng(pdf);
        }
    }
}
