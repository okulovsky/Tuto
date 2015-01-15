using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{




    public class Matcher<TItem,TData>
        where TItem : IItem
    {
        public List<TData> AllExternalDataItems { get; private set; }
        public List<TData> MatchedExternalDataItems { get; private set; }
        public List<TData> UnmatchedExternalDataItems { get; private set; }
        public List<TItem> AllTreeItems { get; private set; }
        public List<TItem> MatchedTreeItems { get; private set; }
        public List<TItem> UnmatchedTreeItems { get; private set; }
        List<TData> initialDataItems;
        Func<TItem, TData, double> distance;
        Func<TData, TData, bool> Equals;

        public Matcher(IEnumerable<TData> allExternal, Func<TItem,TData, double> distance, Func<TData,TData,bool> equals)
        {
            AllExternalDataItems = new List<TData>();
            MatchedTreeItems = new List<TItem>();
            UnmatchedTreeItems = new List<TItem>();
            AllTreeItems = new List<TItem>();
            MatchedExternalDataItems = new List<TData>();
            UnmatchedExternalDataItems = new List<TData>();
            initialDataItems = allExternal.ToList();
			this.distance = distance;
            Equals = equals;
        }


		Dictionary<TItem,TData> match;
		double[,] matrix;

		void MakeMatch(int itemIndex, int dataIndex)
		{
			match[AllTreeItems[itemIndex]] = AllExternalDataItems[dataIndex];
			for (int i = 0; i < matrix.GetLength(0); i++)
				matrix[0, dataIndex] = 0;
			for (int j = 0; j < matrix.GetLength(1); j++)
				matrix[itemIndex, j] = 0;
			UnmatchedExternalDataItems.Remove(AllExternalDataItems[dataIndex]);
			MatchedExternalDataItems.Add(AllExternalDataItems[dataIndex]);
			UnmatchedTreeItems.Remove(AllTreeItems[itemIndex]);
			MatchedTreeItems.Add(AllTreeItems[itemIndex]);
		}

		public void Push(Item root)
		{
			AllExternalDataItems = initialDataItems.ToList();
			MatchedExternalDataItems.Clear();
			UnmatchedExternalDataItems = initialDataItems.ToList();
			AllTreeItems.AddRange(root.Subtree().OfType<TItem>());
			MatchedTreeItems.Clear();
			UnmatchedTreeItems = AllTreeItems.ToList();
			match = new Dictionary<TItem, TData>();
			matrix = new double[AllTreeItems.Count, AllExternalDataItems.Count];

			for (int i = 0; i < AllTreeItems.Count; i++)
				for (int j = 0; j < AllExternalDataItems.Count; j++)
					matrix[i, j] = distance(AllTreeItems[i], AllExternalDataItems[j]);

			for (int i = 0; i < AllTreeItems.Count; i++)
			{
				var storedData = AllTreeItems[i].Get<TData>();
				if (storedData != null)
				{
					var foundData = UnmatchedExternalDataItems.Where(z => Equals(z, storedData)).FirstOrDefault();
					if (foundData != null) MakeMatch(i, AllExternalDataItems.IndexOf(foundData));
					i--;
				}
			}



			while (true)
			{
				int bestX = -1;
				int bestY = -1;
				double best = 0;
				for (int i = 0; i < AllTreeItems.Count; i++)
					for (int j = 0; j < AllExternalDataItems.Count; j++)
						if (bestX < 0 || matrix[bestX, bestY] > best)
						{
							bestX = i;
							bestY = j;
						}
				if (best > 0)
					MakeMatch(bestX, bestY);
				else break;
			}

			DataBinding<TItem>.Pull(root, z => match.ContainsKey(z) ? match[z] : default(TData));
		}

		//TData AccountTreeItems(TItem item)
		//{
		//	AllTreeItems.Add(item);
		//	if (item.Get<TData>() != null) MatchedTreeItems.Add(item);
		//	else UnmatchedTreeItems.Add(item);
		//	return item.Get<TData>();
		//}

		//TData CheckExistingData(TItem item)
		//{
		//	var storedData = item.Get<TData>();
		//	if (storedData == null) return storedData;
		//	var foundData = UnmatchedExternalDataItems.Where(z => Equals(z, storedData)).FirstOrDefault();
		//	if (foundData == null) return foundData;
		//	UnmatchedExternalDataItems.Remove(foundData);
		//	MatchedExternalDataItems.Add(foundData);
		//	return foundData;
		//}

		//TData FoundNewData(TItem item)
		//{
		//	var storedData = item.Get<TData>();
		//	if (storedData != null) return storedData;
		//	var newFoundData = BestMatch(item, UnmatchedExternalDataItems);
		//	if (newFoundData != null)
		//	{
		//		UnmatchedExternalDataItems.Remove(newFoundData);
		//		MatchedExternalDataItems.Add(newFoundData);
		//	}
		//	return newFoundData;

		//}

        
    }
}
