using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static partial class EditorModelIO
    {
        static FileContainer UpdateLocalV1(FileInfo file, string text)
        {
            var container = HeadedJsonFormat.ReadWithoutHeader<FileContainer>(text);
            foreach (var e in container.MontageModel.Information.Episodes)
                e.Guid = Guid.NewGuid();
            return container;
        }
    }
}