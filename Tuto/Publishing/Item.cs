using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
    public enum ItemType
    {
        Folder,
        Lecture,
        Video
    }

    public interface IItem
    {
        Guid Guid { get; }
        string Caption { get; }
        void Store<T>(T data);
        T Get<T>();
    }

    public abstract class Item : NotifierModel, IItem
    {
        public GlobalData GlobalData { get; internal set; }
        public Item Root { get; internal set; }
        public bool IsRoot { get { return Root == this; } }
        public Item Parent { get; internal set; }
        public List<Item> Children { get; private set; }
        public int NumberInTopic { get; internal set; }       
        public abstract Guid Guid { get; }
        public abstract string Caption { get; }
        Dictionary<Type, object> bindedObjects = new Dictionary<Type, object>();

        public Item()
        {
            Children = new List<Item>();
        }
        

        public IEnumerable<Item> PathFromRoot
        {
            get
            {
                if (Parent != null)
                    foreach (var e in Parent.PathFromRoot)
                        yield return e;
                yield return this;

            }
        }

        public IEnumerable<Item> Subtree()
        {
            yield return this;
            foreach (var e in Children)
                foreach (var z in e.Subtree())
                    yield return z;
        }

        public void Store<T>(T value)
        {
            bindedObjects[typeof(T)] = value;
        }

        public T Get<T>()
        {
            if (!bindedObjects.ContainsKey(typeof(T))) return default(T);
            return (T)bindedObjects[typeof(T)];
        }
    }

    public abstract class FolderOrLectureItem : Item
    {
        public Topic Topic { get; internal set; }
        public override Guid Guid
        {
            get { return Topic.Guid; }
        }
        public override string Caption
        {
            get { return Topic.Caption; }
        }
    }

    public class FolderItem : FolderOrLectureItem
    {
    }

    public class LectureItem : FolderOrLectureItem
    {
    }

    public class VideoItem : Item
    {
        public FinishedVideo Video { get; internal set; }
        public override Guid Guid
        {
            get { return Video.Guid; }
        }
        public override string Caption
        {
            get { return Video.Name; }
        }
    }
}