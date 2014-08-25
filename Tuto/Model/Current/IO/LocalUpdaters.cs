using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static partial class EditorModelIO
    {
        private static FileContainer ReadFileContainer(string text)
        {
            var container = HeadedJsonFormat.ReadWithoutHeader<FileContainer>(text);

            return container;
        }

        static FileContainer UpdateLocalV1(FileInfo file, string text)
        {
            var container = ReadFileContainer(text);
            foreach (var e in container.MontageModel.Information.Episodes)
                e.Guid = Guid.NewGuid();
            container.MontageModel.SubtitleFixes = new List<SubtitleFix>();
            return container;
        }



        static FileContainer UpdateLocalV2(FileInfo file, string text)
        {
            var container = ReadFileContainer(text);
            container.MontageModel.SubtitleFixes = new List<SubtitleFix>();
            return container;
        }
    }
}