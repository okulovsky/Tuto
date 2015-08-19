using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.TutoServices.Assembler
{
    class AvsSub : AvsNode
    {
        public AvsNode Payload;
        public string Content;
        public int X;
        public int Y;

        public override void SerializeToContext(AvsContext context)
        {
            base.id = context.Id;
            Payload.SerializeToContext(context);
            var script = string.Format(@"{0} = {1}.Subtitle(""{2}"", x={3}, y={4}, size=24)",Id,Payload.Id,Content, X, Y);
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new[] { Payload }; }
        }
    }
}
