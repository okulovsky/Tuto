using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Tuto.Model;

namespace Tuto.Services
{
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

        public void DoWork(EditorModel model, bool print)
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
            ExecMode mode;
            if (!Enum.TryParse(args[2], true, out mode))
                throw (new ArgumentException(String.Format("Unknown mode: {0}", args[2])));
            var print = mode == ExecMode.Print;

            var model = EditorModelIO.Load(folder);
            DoWork(model, print);
            EditorModelIO.Save(model);
        }
        const string DescriptionString =
@"Muxes audio and video from different sources for assembling.";
        const string HelpString =
@"<folder> <mode>

folder: directory containing video
mode: run or print. Execute commands or write them to stdout";
    }
}
