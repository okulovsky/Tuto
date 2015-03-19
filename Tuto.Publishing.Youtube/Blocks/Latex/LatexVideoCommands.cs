using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing
{
    class LatexVideoCommands : VideoCommandBlockModel<LatexSource,LatexLectureCommands>
    {

        public LatexVideoCommands(LatexSource source, VideoWrap wrap) : base(source,wrap)
        {
            Commands.Add(new VisualCommand(OpenSource, () => LatexSource != null, "edit.png"));
            Commands.Add(new VisualCommand(Compile, () => LatexSource != null, "compile.png"));
            Commands.Add(new VisualCommand(ViewFile, () => LatexSource != null, "file.png"));
            Commands.Add(new VisualCommand(ViewGallery, () => Gallery != null, "view.png"));
        }

        LatexDocument LatexSource { get { return Wrap.Get<LatexDocument>(); } }

        void OpenSource()
        {
            if (LatexSource == null) return;
            if (!LatexSource.OriginalFile.Exists) return;
            Process.Start("\"" + LatexSource.OriginalFile.FullName + "\"");
        }

        void ViewFile()
        {
            if (LatexSource == null) return;
            var file = LatexProcessor.PrepareSeparateSource(LatexSource, Source.LatexFilesStorage);
            Process.Start("\"" + file.FullName + "\"");
        }

        public void Compile()
        {
            if (LatexSource==null) return;
            var pdfFile = LatexProcessor.Compile(LatexSource, Source.LatexFilesStorage);
            GalleryInfo galleryData = null;
            if (pdfFile != null)
            {
                pdfFile.CopyTo(PdfFile.FullName, true);
                //var directory = DueSlidesDirectory;
                //if (directory.Exists) directory.Delete(true);
                //directory.Create();
                //LatexProcessor.ConvertToPng(pdfFile, directory;)
                galleryData = new GalleryInfo { CompilationTime = DateTime.Now };
            }
            Wrap.Store(galleryData);
            MakeChange();
        }

        void ViewGallery()
        {
            if (Gallery == null) return;
            var path = System.IO.Path.Combine(Source.LatexSlidesStorage.FullName, Wrap.Video.Guid.ToString());
            if (!Directory.Exists(path)) return;
            Process.Start("\"" + path + "\"");
        }

        public DirectoryInfo OutputDirectory
        {
            get
            {
                return Source.LatexSlidesStorage.CreateSubdirectory(Wrap.Video.Guid.ToString());
            }
        }

        public DirectoryInfo DueSlidesDirectory
        {
            get { return OutputDirectory.CreateSubdirectory("pngs"); }
        }

        public FileInfo PdfFile
        {
            get { return new FileInfo(Path.Combine(OutputDirectory.FullName, "slides.pdf")); }
        }


        public override string ImageFileName
        {
            get { return "latex.png"; }
        }

        GalleryInfo Gallery
        {
            get { return Wrap.Get<GalleryInfo>(); }
        }

        override public IEnumerable<BlockStatus> Status
        {
            get
            {
				if (LatexSource == null)
					yield return BlockStatus.OK("No LaTeX source is found for this video").WithBrush(Brushes.LightGray).PreventExport();
				else if (Gallery == null)
					yield return BlockStatus.Auto("Slides were not compiled").PreventExport();
				//if (!DueSlidesDirectory.Exists) return BlockStatus.Error("Slides were compiled, but they are not found now. Recompile them");
				//if (Gallery.CompilationTime < LatexSource.ModificationTime) return BlockStatus.Error("Slides are outdated. Recompile them");
				//if (DueSlidesDirectory.GetFiles().Length == 0) return BlockStatus.Warning("No slides are produced for this video. Check the presentation and remove the section, if it was intended");
				else if (!PdfFile.Exists)
					yield return BlockStatus.Auto("Slides were compiled, but they are not found now. Recompile them").PreventExport();
				else if (Gallery.CompilationTime < LatexSource.ModificationTime)
					yield return BlockStatus.Auto("Slides are outdated. Recompile them").PreventExport();
				else
					yield return BlockStatus.OK();
            }
        }

		public override void TryMakeItRight()
		{
			if (Status.StartAutoCorrection())
				Compile();
		}
    }
}
