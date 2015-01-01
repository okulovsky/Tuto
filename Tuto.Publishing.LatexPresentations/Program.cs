using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuto.Publishing.LatexPresentations
{
    class Program
    {
        static void Main(string[] args)
        {
            var latexFile = new FileInfo(@"..\..\..\..\AIML\Latex\ulearn-lecture-01.tex");
            var processor = new LatexProcessor();
            var document = processor.Parse(latexFile);
            processor.Compile(document, latexFile.Directory);
        }
    }
}
