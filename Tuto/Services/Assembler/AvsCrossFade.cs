namespace Tuto.Services.Assembler
{
    class AvsCrossFade : AvsNode
    {
        public AvsNode FadeFrom { get; set; }

        public AvsNode FadeTo { get; set; }

        public int Duration = 500;

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            FadeFrom.SerializeToContext(context);
            FadeTo.SerializeToContext(context);
            var from = FadeFrom.Id;
            var to = FadeTo.Id;
            var script = string.Format(Format, Id, to, from, Duration);
            context.AddData(script);
        }

        protected override string Format { get { return "{0} = {2} + CrossFadeTime({1}, {2}, {3})"; } }
    }
}