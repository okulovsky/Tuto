using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tuto.Services.Assembler
{
    class AvsWatermark : AvsNode
    {
        public AvsNode Payload { get; set; }

        public FileInfo ImageFile { get; set; }

        public int X { get; set; } 

        public int Y { get; set; } 

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Payload.SerializeToContext(context);
            var video = Payload.Id;
            var script = string.Format(Format, Id, video, ImageFile, X, Y);
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new[] { Payload }; }
        }

        protected override string Format { get { return "{0} = AddWatermarkPNG({1}, \"{2}\", {3}, {4})"; } }
    }
}