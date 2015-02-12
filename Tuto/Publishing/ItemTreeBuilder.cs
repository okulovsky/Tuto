using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
    public static class ItemTreeBuilder
    {
        public static Item Build<TFolderItem, TLectureItem, TVideoItem>(CourseTreeData globalData)
            where TFolderItem : FolderItem, new()
            where TLectureItem : LectureItem, new()
            where TVideoItem : VideoItem, new()
        {
            var result = BuildTopic<TFolderItem, TLectureItem, TVideoItem>(globalData, globalData.Structure.RootTopic);
            result.Root = result;
            lectureNumber = 0;
            InitializeItem(result);
            return result;
        }

       static int lectureNumber = 0;

        public static Item BuildTopic<TFolderItem, TLectureItem, TVideoItem>(CourseTreeData globalData, Topic topic)
            where TFolderItem : FolderItem, new()
            where TLectureItem : LectureItem, new()
            where TVideoItem : VideoItem, new()
        {
            Item item = null;
            var videos = globalData.Structure.VideoToTopicRelations.Where(z => z.TopicGuid == topic.Guid).OrderBy(z=>z.NumberInTopic).ToList();
            
			
			if (videos.Count != 0 && topic.Items.Count != 0)
                throw new Exception("Topic " + topic.Caption + " contains both videos and subtopics");

            if (videos.Count == 0)
            {
                item = new TFolderItem() { Topic=topic };
                foreach (var e in topic.Items)
                {
                    item.Children.Add(BuildTopic<TFolderItem, TLectureItem, TVideoItem>(globalData, e));
                }
                return item;
            }

            item = new TLectureItem() { Topic=topic };
            foreach (var e in videos)
            {
				var video = globalData.Videos.Where(z => z.Guid == e.VideoGuid).FirstOrDefault();
				if (video == null) continue;
                item.Children.Add(new TVideoItem { Video = video });
            }
            return item;
        }

        public static void InitializeItem(Item item)
        {
            var lecture = item as LectureItem;
            if (lecture != null)
                lecture.LectureNumber = lectureNumber++;

            for (int i = 0; i < item.Children.Count; i++)
            {
                var e = item.Children[i];
                e.Parent = item;
                e.NumberInTopic = i;
                e.Root = item.Root;
                InitializeItem(e);
            }
        }
    }
}
