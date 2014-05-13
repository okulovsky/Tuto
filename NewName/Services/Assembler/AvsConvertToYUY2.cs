namespace NewName.Services.Assembler
{
    class AvsConvertToYUY2 : AvsNode
    {
        public AvsNode Payload { get; set; }

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Payload.SerializeToContext(context);
            var video = Payload.Id;
            var script = string.Format(Format, Id, video);
            context.AddData(script);
        }

        protected override string Format
        {
            get { return "{0} = ConvertToYUY2({1})"; }
        }
    }
}