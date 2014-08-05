using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.TutoServices.Assembler
{
    internal abstract class AvsNode
    {
        public abstract void SerializeToContext(AvsContext context);

        public abstract IList<AvsNode> ChildNodes { get; }

        public string Id
        {
            get
            {
                if(id == BadId)
                    throw new ApplicationException("Requested AvsNode is not serialized");
                return String.Format(Template, id);
            }
        }

        protected int id = BadId;

        private static int BadId
        {
            get { return -1; }
        }

        public static string Template
        {
            get { return "var_{0}"; }
        }

        protected virtual string Format{get { return ""; }}

        public static AvsNode NormalizedNode(FileInfo chunkFile, bool autolevel)
        {
            var chunk = new AvsChunk {ChunkFile = chunkFile};
			
            return NormalizedNode(chunk, autolevel);
        }

        public static AvsNode NormalizedNode(AvsNode node, bool autolevel=false)
        {
			var trimmed = new AvsTrimToVideoLength { Payload = node };
	        return trimmed;


	        //var resized = new AvsResize { Payload = yuy2 };
	        //var changedFps = new AvsChangeFramerate { Payload = resized };
	        //return changedFps;

	        /*
            if(!autolevel)
                return changedFps;
            var leveled = new AvsAutoLevels {Payload = changedFps};
            return leveled;
			*/
        }
    }
}
