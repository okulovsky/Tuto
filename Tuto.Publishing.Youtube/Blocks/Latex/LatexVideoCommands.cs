using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tuto.Publishing
{
    class LatexVideoCommands : CommandsBlockModel<LatexSource,VideoWrap>
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
                var directory = Source.LatexSlidesStorage.CreateSubdirectory(Wrap.Video.Guid.ToString());
                if (directory.Exists) directory.Delete(true);
                directory.Create();
                LatexProcessor.ConvertToPng(pdfFile, directory);
                galleryData = new GalleryInfo { CompilationTime = DateTime.Now, Directory = directory };
            }
            Wrap.Store(galleryData);
            Source.Save(Wrap.Root);
            this.NotifyByExpression(z => z.Status);
        }

        void ViewGallery()
        {
            if (Gallery == null) return;
            var path = System.IO.Path.Combine(Source.LatexSlidesStorage.FullName, Wrap.Video.Guid.ToString());
            if (!Directory.Exists(path)) return;
            Process.Start("\"" + path + "\"");
        }

        public override string ImageFileName
        {
            get { return "latex.png"; }
        }

        GalleryInfo Gallery
        {
            get { return Wrap.Get<GalleryInfo>(); }
        }

        override public System.Windows.Media.Brush Status
        {
            get
            {
                if (LatexSource == null) return Brushes.Gray;
                if (Gallery==null || !Gallery.Directory.Exists || Gallery.CompilationTime<LatexSource.ModificationTime) return Brushes.Red;
                return Brushes.Green;
            }
        }
    }
}
