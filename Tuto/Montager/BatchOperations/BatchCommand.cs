using System.IO;

namespace Montager
{

    public class BatchCommandContext
    {
        public string path;
        public StreamWriter batFile;
        public bool lowQuality;
    }

    public abstract class BatchCommand
    {
        public abstract string Caption { get; }
        public abstract void WriteToBatch(BatchCommandContext context);
    }


}
