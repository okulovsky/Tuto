using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator
{
    public enum MoveDestination
    {
        Into,
        Before,
        After,
    }

    public class PublishViewModel
    {
        public TopicWrap[] Root { get; private set; }
        public ObservableCollection<VideoWrap> UnassignedVideos { get; private set; }
        public GlobalData GlobalData { get; private set; }

        public PublishViewModel(GlobalData globalData)
        {
            this.GlobalData = globalData;
            Root = new TopicWrap[] { new TopicWrap(globalData.TopicsRoot) };
            UnassignedVideos = new ObservableCollection<VideoWrap>();
            foreach (var e in GlobalData.VideoData.Where(z => z.TopicGuid == Guid.Empty))
                UnassignedVideos.Add(new VideoWrap(e));
        }

        //void MoveTopic(TopicViewModel what, TopicViewModel where, int index)
        //{
        //    //проверяем, что не переносим корень
        //    if (what.Parent == null) return;

            

        //    //проверяем, что не переносим родителя в ребенка
        //    var parent=where;
        //    while (true)
        //    {
        //        if (what == parent) return;
        //        parent=parent.Parent;
        //        if (parent == null) break;
        //    }

        //    what.Parent.Items.Remove(what);
        //    if (index < where.Items.Count)
        //        where.Items.Insert(index, what);
        //    else if (index == where.Items.Count)
        //        where.Items.Add(what);
        //    else throw new ArgumentException();
        //    what.Parent = where;
        //}

        bool CheckMoveConsistancy(Wrap what, Wrap where)
        {
            //Корень перемещать нельзя
            if (what == Root[0]) return false;

            //Нельзя перемещать в сына
            var parent = where;
            while (true)
            {
                if (parent == null) break;
                if (parent == what) return false;
                parent = parent.Parent;
            }

            //Нельзя перемещать в видео
            if (where is VideoWrap) return false;

            return true;
        }

        void Remove(Wrap what)
        {
            if (what.Parent == null) return;
            what.Parent.Items.Remove(what);
            what.Parent = null;
        }

        void Delete(Wrap wrap)
        {
            foreach (var e in wrap.Subtree.OfType<VideoWrap>()) UnassignedVideos.Add(e);
        }

        public void Insert(Wrap what, Wrap where, Wrap beforeOf)
        {
            var index = where.Items.IndexOf(beforeOf);
            if (index == -1) index = where.Items.Count;
            where.Items.Insert(index, what);
            what.Parent = where;
        }

        Tuple<Wrap, Wrap> GetWhereAndBeforeOf(Wrap what, Wrap anchor, MoveDestination dst)
        {

            if (dst == MoveDestination.Into)
            {
                if (what is VideoWrap && anchor is VideoWrap)
                    return GetWhereAndBeforeOf(what, anchor, MoveDestination.After);
                return new Tuple<Wrap, Wrap>(anchor, null);
            }

            if (dst == MoveDestination.Before)
            {
                return Tuple.Create(anchor.Parent, anchor);
            }

            if (dst == MoveDestination.After)
            {
                if (anchor.Items.Count == 0)
                {
                    if (anchor.Parent == null) return new Tuple<Wrap, Wrap>(null, null);
                    var index = anchor.Parent.Items.IndexOf(anchor);
                    if (index == anchor.Parent.Items.Count - 1)
                        return new Tuple<Wrap, Wrap>(anchor.Parent, null);
                    return
                        Tuple.Create(anchor.Parent, anchor.Parent.Items[index + 1]);
                }
                return Tuple.Create(anchor, anchor.Items[0]);
            }

            throw new Exception("Cannot fall here");
        }

        public void MoveToTree(Wrap what, Wrap anchor, MoveDestination dst)
        {
            var resp = GetWhereAndBeforeOf(what, anchor, dst);
            Wrap where = resp.Item1;
            Wrap beforeOf = resp.Item2;


            if (where == null) return;
            if (!CheckMoveConsistancy(what, where)) return;

            if (what.Parent!=null)
                Remove(what);
            if (UnassignedVideos.Contains(what))
                UnassignedVideos.Remove(what as VideoWrap);


            Insert(what, where, beforeOf);
        }
    }
}
