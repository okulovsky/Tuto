using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Tuto.Model;

namespace Tuto.TutoServices.Montager
{
    class ProcessingCommands
    {   

        public static IEnumerable<FFMPEGCommand> Processing(EditorModel model, List<MontageChunk> chunks)
        {
            return chunks.SelectMany(z => Commands(model, z));
        }

        public static IEnumerable<FFMPEGCommand> Commands(EditorModel model, MontageChunk chunk)
        {
            return null;
        }
    }
}
