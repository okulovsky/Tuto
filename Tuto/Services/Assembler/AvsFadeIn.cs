using System.Collections.Generic;

namespace Tuto.TutoServices.Assembler
{
    public class AvsFadeIn : AvsNode
    {
        public AvsNode Payload { get; set; }

        public int Duration = 500;

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Payload.SerializeToContext(context);
            var video = Payload.Id;
            var script = string.Format(Format, Id, video, Duration);
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new[] { Payload }; }
        }

        protected override string Format { get { return "{0} = FadeInTime({1}, {2})"; } }
    }
}