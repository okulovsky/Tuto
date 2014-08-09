using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator
{
    public class PublishViewModel
    {
        public TopicViewModel[] Root { get; private set; }
        public ObservableCollection<PublishVideoData> NotDistributedVideos { get; private set; }
        public GlobalData GlobalData { get; private set; }

        TopicViewModel Convert(Topic topic)
        {
            var result = new TopicViewModel()
            {
                Guid = topic.Guid,
                Caption = topic.Caption,
            };
            foreach (var e in topic.Items.Select(z => Convert(z)))
            {
                e.Parent = result;
                result.Items.Add(e);
            }
            foreach (var e in GlobalData.VideoData.Where(z => z.TopicGuid == result.Guid).OrderBy(z => z.NumberInTopic))
            {
                result.Items.Add(e);
            }
            return result;
        }

        public PublishViewModel(GlobalData globalData)
        {
            this.GlobalData = globalData;
            Root = new TopicViewModel[] { Convert(globalData.TopicsRoot) };
            NotDistributedVideos = new ObservableCollection<PublishVideoData>();
            foreach (var e in GlobalData.VideoData.Where(z => z.TopicGuid == null))
                NotDistributedVideos.Add(e);
        }



  

    }
}
