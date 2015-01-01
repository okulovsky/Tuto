using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{

    public static class DataBinding<TItem>
        where TItem : IItem
    {
        public static void Pull<TData>(Item root, Func<TItem, TData> dataSource)
        {
            var itemParam = Expression.Parameter(typeof(TItem), "item");
            var dataParam = Expression.Parameter(typeof(TData), "data");
            foreach (var e in root.Subtree().OfType<TItem>())
            {
                var data = dataSource(e);
                if (e != null) e.Store(data);
            }
        }

        public static void PullFromLayer<TData>(Item root, DataLayer<TData> layer)
        {
            Pull(root, item => layer.Records.Where(z => z.Guid == item.Guid).Select(z => z.Data).FirstOrDefault());
        }

        public static string GetHeader<TData>()
        {
            return "Tuto layer for " + GetName<TData>();
        }

        public static string GetName<TData>()
        {
            return typeof(TData).Name;
        }

        public static string GetFileName<TData>()
        {
             return GetName<TData>() + ".layer.txt";
        }

        public static void PullFromFile<TData>(Item root, DirectoryInfo directory)
        {
            var fileName=GetFileName<TData>();   
            var file = directory.GetFiles(fileName).FirstOrDefault();
            if (file==null)
                return;
            var layer = HeadedJsonFormat.Read<DataLayer<TData>>(file, GetHeader<TData>() , 1);
            Pull(root, item => layer.Records.Where(z => z.Guid == item.Guid).Select(z => z.Data).FirstOrDefault());
        }

        public static DataLayer<TData> GetLayer<TData>(Item root)
        {
            var layer = new DataLayer<TData>();
            foreach (var e in root.Subtree().OfType<TItem>())
            {
                var data = e.Get<TData>();
                if (data != null)
                    layer.Records.Add(new DataLayerRecord<TData>(e.Guid, data));
            }
            return layer;
        }

        public static void SaveLayer<TData>(Item root, DirectoryInfo directory)
        {
            var fileName = GetFileName<TData>();
            var file = new FileInfo(Path.Combine(directory.FullName,fileName));
            var layer = GetLayer<TData>(root);
            HeadedJsonFormat.Write(file, GetHeader<TData>(), 1, layer);
        }
    }
}
