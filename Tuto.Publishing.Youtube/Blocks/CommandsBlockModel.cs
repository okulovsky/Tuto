using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    public abstract class CommandsBlockModel<TSource,TItem> : NotifierModel, ICommandBlockModel
        where TItem : Item
    {
        protected readonly TItem Wrap;
        protected readonly TSource Source;

        public CommandsBlockModel(TSource source, TItem item)
        {

            Wrap = item;
            Source = source;
            Commands = new List<VisualCommand>();
        }
        
        public List<VisualCommand> Commands
        {
            get;
            private set; 
        }

        public abstract string ImageFileName { get; }

		public virtual void TryMakeItRight() { }

        public Uri ImageSource
        {
            get { return new Uri("/Img/" + ImageFileName, UriKind.Relative); }
        }

        public abstract BlockStatus Status { get; }

      
    }

    public abstract class LectureCommandBlockModel<TSource, TVideoData> : CommandsBlockModel<TSource, LectureWrap>
    {
        public LectureCommandBlockModel(TSource source, LectureWrap item) : base(source, item) { }
        public IEnumerable<TVideoData> VideoData { get { return Wrap.Subtree().OfType<VideoWrap>().SelectMany(z => z.CommandBlocks.OfType<TVideoData>()); } }
    }

    public abstract class VideoCommandBlockModel<TSource, TLectureData> : CommandsBlockModel<TSource, VideoWrap>
        where TSource : IMaterialSource
        where TLectureData : NotifierModel, ICommandBlockModel
    {
        public VideoCommandBlockModel(TSource source, VideoWrap item) : base(source, item) { }

        public TLectureData LectureData
        {
            get
            {
                return (Wrap.Parent as LectureWrap).CommandBlocks.OfType<TLectureData>().FirstOrDefault();
            }
        }

        public void MakeChange()
        {
            base.Source.Save(Wrap.Root);
            this.NotifyByExpression(z => z.Status);
            LectureData.NotifyByExpression(z => z.Status);
        }
    }
}
