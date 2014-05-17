namespace Tuto.Services.Assembler
{
    class AvsResize : AvsNode
    {
        public AvsNode Payload { get; set; }
        public int Width = 1280;
        public int Height = 720;

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Payload.SerializeToContext(context);
            var video = Payload.Id;
            var script = string.Format(Format, Id, video, Width, Height);
            context.AddData(script);
        }

        protected override string Format { get { return "{0} = BilinearResize({1}, {2}, {3})"; } }
    }
}