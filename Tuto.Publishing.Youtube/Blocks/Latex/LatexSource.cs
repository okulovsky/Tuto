using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
    class LatexSource : IMaterialSource
    {
        public DirectoryInfo LatexFilesStorage { get; private set; }
        public DirectoryInfo LatexSlidesStorage { get; private set; }
        public PublishingSettings Settings { get; private set; }

        public void Initialize(PublishingSettings settings)
        {
            Settings = settings;
            LatexFilesStorage = settings.Location.CreateSubdirectory(settings.LatexSourceSubdirectory);
            LatexSlidesStorage = settings.Location.CreateSubdirectory(settings.LatexCompiledSlidesSubdirectory);
            if (!LatexSlidesStorage.Exists) LatexSlidesStorage.Create();
        }

        public void Load(Item root)
        {
            Pull(root);
            DataBinding<VideoWrap>.PullFromFile<GalleryInfo>(root, Settings.Location);
        }

        public void Pull(Item root)
        {
            var documents = LatexProcessor.GetAllPresentations(LatexFilesStorage);
            var matcher = Matchers.ByName<VideoWrap, LatexDocument>(documents, doc => doc.LastSection.Name, (doc1, doc2) => doc1 == doc2);
            matcher.Push(root);
        }

        public void Save(Item root)
        {
            DataBinding<VideoWrap>.SaveLayer<GalleryInfo>(root, Settings.Location);
        }

        public ICommandBlockModel ForVideo(VideoWrap wrap)
        {
            return new LatexVideoCommands(this, wrap);
        }

        public ICommandBlockModel ForLecture(LectureWrap wrap)
        {
            return new LatexLectureCommands(this, wrap);
        }
    }
}
