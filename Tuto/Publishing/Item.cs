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
    }

    public abstract class Item : IItem
    {
        public Item Root { get; internal set; }
        public Item Parent { get; internal set; }
        public List<Item> Children { get; internal set; }
        public int NumberInTopic { get; internal set; }
        public abstract Guid Guid { get; }

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
    }

    public abstract class FolderOrLectureItem : Item
    {
        public Topic Topic { get; internal set; }
        public override Guid Guid
        {
            get { return Topic.Guid; }
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
    }
}