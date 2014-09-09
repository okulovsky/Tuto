using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

namespace Tuto.TutoServices.Assembler
{
    class AvsChunk : AvsNode
    {
        public FileInfo ChunkFile { get; set; }
        public int ConvertToFps { get; set; }

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            context.AddData(String.Format(Format, Id, ChunkFile, ConvertToFps));
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new AvsNode[] {}; }
        }

        protected override string Format 
        {
            get { return "{0} = DirectShowSource(\"{1}\", convertFps=True, fps={2})"; }
        }
    }
}