using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator
{
    public abstract class Wrap
    {
        public Wrap Parent { get; set; }
        public ObservableCollection<Wrap> Items { get; private set; }
        public Wrap()
        {
            Items=new ObservableCollection<Wrap>();
        }
        public IEnumerable<Wrap> Subtree
        {
            get
            {
                yield return this;
                foreach (var e in Items) yield return e;
            }
        }
    }

    public class TopicWrap : Wrap
    {
        public Topic Topic { get; private set; }

        public TopicWrap(Topic topic)
        {
            this.Topic = topic;
            foreach (var e in topic.Items)
            {
                var child = new TopicWrap(e);
                Items.Add(child);
                child.Parent = this;
            }
        }
    }

    public class VideoWrap : Wrap
    {
        public PublishVideoData Video { get; private set; }
        public VideoWrap(PublishVideoData data)
        {
            Video = data;
        }
    }
}
