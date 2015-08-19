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
        public double Start;
        public double End;

        public override void SerializeToContext(AvsContext context)
        {
            base.id = context.Id;
            Payload.SerializeToContext(context);
            var script = string.Format(@"{0} = {1}.Subtitle(""{2}"", x={3}, y={4}, size=24, first_frame={5}, last_frame={6})",Id,Payload.Id,Content, X, Y, (int)(Start*25), (int)(End*25));
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new[] { Payload }; }
        }
    }
}
