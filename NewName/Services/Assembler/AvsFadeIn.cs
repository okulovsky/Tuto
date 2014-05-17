namespace Tuto.Services.Assembler
{
    class AvsFadeIn : AvsNode
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

        protected override string Format { get { return "{0} = FadeInTime({1}, {2})"; } }
    }
}