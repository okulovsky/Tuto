using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuto.Services.Assembler
{
    class AvsConcatList : AvsNode
    {
        public List<AvsNode> Items { get; set; }

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Items.ForEach(item => item.SerializeToContext(context));
            var allItems = string.Join(" + ", Items.Select(item => item.Id));
            context.AddData(string.Format(Format, Id, allItems));
        }

        protected override string Format { get { return "{0} = {1}"; } }
    }
}