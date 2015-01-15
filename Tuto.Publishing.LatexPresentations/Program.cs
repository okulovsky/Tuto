using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tuto.Model;

namespace Tuto.Publishing.LatexPresentations
{
    [DataContract]
    class GalleryInfo
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DirectoryInfo Directory { get; set; }
    }

    class Program
    {
        public const string Pdflatex = "pdflatex";
        public const string Ghostscript = @"C:\Program Files\gs\gs9.15\bin\gswin64.exe";
        
     
        static void Main(string[] args)
        {
            var directory = new DirectoryInfo(args[0]);
            var slides=directory.CreateSubdirectory("LaTeXCompiledSlides");
            slides.Delete(true);
            slides.Create();

			var latexDirectory = directory.CreateSubdirectory("LaTeX");
            var files = latexDirectory.GetFiles("L*.tex");
            var galleries = new List<GalleryInfo>();
            var processor = new LatexProcessor();

            foreach (var file in files)
            {
                var doc = processor.Parse(file);
                var docs = doc.Sections.Select(z => new LatexDocument { Preamble = doc.Preamble, Sections = new List<LatexSection> { z } }).ToList();
                int number = 0;
                foreach (var e in docs)
                {
                    var pdf = processor.Compile(e, latexDirectory);
                    var targetDirectory = slides.CreateSubdirectory(file.Name + "." + number);
                    processor.ConvertToPng(pdf, targetDirectory);
                    galleries.Add(new GalleryInfo { Name = e.LastSection.Name, Directory=targetDirectory });
                    number++;
                }
            }

            var matcher = 
                Matcher.ByName<VideoItem, GalleryInfo>(galleries, z => z.Name, (a, b) => a.Directory.FullName == b.Directory.FullName);

            var model = EditorModelIO.ReadGlobalData(directory);
            var root = ItemTreeBuilder.Build<FolderItem, LectureItem, VideoItem>(model);
            matcher.Push(root);
            DataBinding<VideoItem>.SaveLayer<GalleryInfo>(root, directory);
        }
    }
}
