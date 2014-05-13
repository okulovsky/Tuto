using System;
using System.IO;
using System.Web.UI.WebControls;

namespace NewName.Services.Assembler
{
    class AvsChunk : AvsNode
    {
        public FileInfo ChunkFile { get; set; }

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            context.AddData(String.Format(Format, Id, ChunkFile));
        }

        protected override string Format 
        {
            get { return "{0} = DirectShowSource(\"{1}\")"; }
        }
    }
}