using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto;
using Tuto.Model;

namespace Tuto.TutoServices.Assembler
{
    public abstract class AvsNode
    {
        public abstract void SerializeToContext(AvsContext context);

        public abstract IList<AvsNode> ChildNodes { get; }


        public int SyncShift = 0;
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

        public static AvsNode NormalizedNode(StreamChunk chunkFile, int fps, bool autolevel, int shift)
        {
            var chunk = new AvsChunk {Chunk = chunkFile, ConvertToFps = fps};
            chunk.SyncShift = shift;
            return NormalizedNode(chunk, autolevel);
        }

        public static AvsNode NormalizedNode(AvsNode node, bool autolevel=false)
        {
            return node;
            //var sameLen = new AssumeSameAVLength { Payload = node };
            //return sameLen;
        }
    }
}
