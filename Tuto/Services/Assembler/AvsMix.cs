using System.Collections.Generic;

namespace Tuto.TutoServices.Assembler
{
    public class AvsMix : AvsNode
    {
        public AvsNode First { get; set; } //video

        public AvsNode Second { get; set; } //audio

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            First.SerializeToContext(context);
            Second.SerializeToContext(context);
            var script = string.Format(Format, Id, First.Id, Second.Id);
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new[] { First, Second }; }
        }

        protected override string Format { get { return "{0} = audiodub({1}.KillAudio(), {2}.KillVideo())"; } }
    }
}