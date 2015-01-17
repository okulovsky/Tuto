using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    class LatexSource : IMaterialSource
    {
        DirectoryInfo latexFilesStorage;

        public void Initialize(Model.GlobalData data)
        {
            latexFilesStorage = data.GlobalDataFolder.CreateSubdirectory("Latex");
        }

        public void Load(Item root)
        {
            Pull(root);
        }

        public void Pull(Item root)
        {
            var documents = LatexProcessor.GetAllPresentations(latexFilesStorage);
            var matcher = Matchers.ByName<VideoWrap, LatexDocument>(documents, doc => doc.LastSection.Name, (doc1, doc2) => doc1 == doc2);
            matcher.Push(root);
        }

        public void Save(Item root)
        {
            
        }

        public ICommandBlockModel ForVideo(VideoWrap wrap)
        {
            throw new NotImplementedException();
        }

        public ICommandBlockModel ForLecture(LectureWrap wrap)
        {
            throw new NotImplementedException();
        }
    }
}
