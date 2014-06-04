using System.Collections.Generic;

namespace Tuto.Services.Assembler
{
    class AvsConcatTwo : AvsNode
    {
        public AvsNode First { get; set; }

        public AvsNode Second { get; set; }

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

        protected override string Format { get { return "{0} = {1} + {2}"; } }
    }
}