using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuto.TutoServices.Assembler
{
    class AvsConcatList : AvsNode
    {
        public List<AvsNode> Items { get; set; }

	    public List<AvsContext> SerializeToMultipleContexts(int count)
	    {
			var contexts = new List<AvsContext>();

		    var contextLists = ChunkifyList(ChildNodes, count);
		    foreach (var contextList in contextLists)
		    {
			    var concat = new AvsConcatList {Items = contextList};
			    var context = new AvsContext();
				concat.SerializeToContext(context);
				contexts.Add(context);
		    }
		    return contexts;
	    } 

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            Items.ForEach(item => item.SerializeToContext(context));
            var allItems = string.Join(" + ", Items.Select(item => item.Id));
            context.AddData(string.Format(Format, Id, allItems));
        }

	    private static IEnumerable<List<AvsNode>> ChunkifyList(IList<AvsNode> list, int count)
	    {
		    var listCount = (int) Math.Ceiling(Decimal.Divide(list.Count, count));
		    for (var i = 0; i < listCount; i++)
			    yield return list.Skip(i*count).Take(count).ToList();
	    }

        public override IList<AvsNode> ChildNodes
        {
            get { return Items.AsReadOnly(); }
        }

        protected override string Format { get { return "{0} = {1}"; } }
    }
}