using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace Tuto.Services
{

    enum MontagerMode
    {
        Run, Print
    }

    class MontagerService : Service
    {

        public override string Name
        {
            get { return "montager"; }
        }

        public override string Description
        {
            get { return DescriptionString; }
        }

        public override string Help
        {
            get { return HelpString; }
        }

        public void DoWork(EditorModelV4 model, bool print)
        {
            model.ChunkFolder.Delete(true);
            model.ChunkFolder.Create();
            foreach (var e in Montager.ProcessingCommands.Processing(model, model.Montage.FileChunks))
            {
                e.Execute(print);
            }
        }

        public override void DoWork(string[] args)
        {
            if(args.Length < 3)
                throw (new ArgumentException(String.Format("Insufficient args")));
            var folder = args[1];
            MontagerMode mode;
            if (!Enum.TryParse(args[2], true, out mode))
                throw (new ArgumentException(String.Format("Unknown mode: {0}", args[2])));
            var print = mode == MontagerMode.Print;

            var model = ModelIO.Load(folder);
            DoWork(model, print);
            ModelIO.Save(model);
        }
        const string DescriptionString =
@"MontagerService service. Muxes audio and video from different sources to chunks for assembling.";
        const string HelpString =
@"<folder> <mode> [fast]

folder: directory containing video
mode: run or print. Execute commands or write them to stdout";
    }
}
