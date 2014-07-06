using System;
using System.Collections.Generic;

namespace Tuto.TutoServices.Assembler
{
    class AvsAutoLevels : AvsNode
    {
        // docs at http://thebattles.net/video/autolevels.html

        public AvsNode Payload { get; set; }



        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Payload.SerializeToContext(context);
            var video = Payload.Id;
            var script = string.Format(Format, Id, video);
            context.AddData(script);
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new [] {Payload}; }
        }

        protected override string Format
        {
            get { return "{0} = Autolevels({1})"; }
        }
    }
}