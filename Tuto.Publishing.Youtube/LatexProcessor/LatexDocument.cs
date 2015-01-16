using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuto.Publishing
{
    class LatexSection
    {
        public string Name;
        public List<LatexSlide> Slides = new List<LatexSlide>();
        public LatexSlide LastSlide { get { if (Slides.Count != 0) return Slides[Slides.Count - 1]; return null; } }
    }

    class LatexSlide
    {
        public string Content;
    }

    class LatexDocument
    {
        public string Preamble;
        public List<LatexSection> Sections = new List<LatexSection>();
        public LatexSection LastSection { get { if (Sections.Count != 0) return Sections[Sections.Count - 1]; return null; } }
    }
}
