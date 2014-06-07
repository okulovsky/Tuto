using System.Collections.Generic;

namespace Tuto.Services.Assembler
{
    class AvsChangeFramerate : AvsNode
    {
        public AvsNode Payload { get; set; }

        public double FPS = 30.0;

        public int Zone = 80;

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Payload.SerializeToContext(context);
            var video = Payload.Id;
            var script = string.Format(Format, Id, video, FPS, Zone);
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new[] { Payload }; }
        }

        protected override string Format
        {
            get { return "{0} = ConvertFPS({1}, {2}, zone={3})"; }
        }
    }
}