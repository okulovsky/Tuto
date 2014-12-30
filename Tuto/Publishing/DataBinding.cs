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
        public static void Pull<TData>(Item root, Expression<Func<TItem, TData>> field, Func<TItem, TData> dataSource)
        {
            var itemParam = Expression.Parameter(typeof(TItem), "item");
            var dataParam = Expression.Parameter(typeof(TData), "data");

            var lambda = Expression.Lambda<Action<TItem, TData>>(
                Expression.Assign(
                    Expression.MakeMemberAccess(
                        itemParam,
                        (field.Body as MemberExpression).Member),
                    dataParam),
                itemParam,
                dataParam).Compile();
            foreach (var e in root.Subtree().OfType<TItem>())
            {
                var data = dataSource(e);
                if (e != null)
                    lambda(e, data);
            }
        }

        public static void PullFromLayer<TData>(Item root, Expression<Func<TItem, TData>> field, DataLayer<TData> layer)
        {
            Pull(root, field, item => layer.Records.Where(z => z.Guid == item.Guid).Select(z => z.Data).FirstOrDefault());
        }

        public static string GetHeader<TData>(Expression<Func<TItem, TData>> field)
        {
            return "Tuto layer for " + GetName(field);
        }

        public static string GetName<TData>(Expression<Func<TItem, TData>> field)
        {
            return (field.Body as MemberExpression).Member.Name;
        }

        public static string GetFileName<TData>(Expression<Func<TItem, TData>> field)
        {
             return GetName(field) + ".layer.txt";
        }

        public static void PullFromFile<TData>(Item root, Expression<Func<TItem, TData>> field, DirectoryInfo directory)
        {
            var fileName=GetFileName(field);   
            var file = directory.GetFiles(fileName).FirstOrDefault();
            if (file==null)
                return;
            var layer = HeadedJsonFormat.Read<DataLayer<TData>>(file, GetHeader(field) , 1);
            Pull(root, field, item => layer.Records.Where(z => z.Guid == item.Guid).Select(z => z.Data).FirstOrDefault());
        }

        public static DataLayer<TData> GetLayer<TData>(Item root, Func<TItem, TData> field)
        {
            var layer = new DataLayer<TData>();
            foreach (var e in root.Subtree().OfType<TItem>())
            {
                var data = field(e);
                if (data != null)
                    layer.Records.Add(new DataLayerRecord<TData>(e.Guid, data));
            }
            return layer;
        }

        public static void SaveLayer<TData>(Item root, Expression<Func<TItem, TData>> field, DirectoryInfo directory)
        {
            var fileName = GetFileName(field);
            var file = new FileInfo(Path.Combine(directory.FullName,fileName));
            var layer = GetLayer(root, field.Compile());
            HeadedJsonFormat.Write(file, GetHeader(field), 1, layer);
        }
    }
}
