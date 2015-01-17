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

        public Uri ImageSource
        {
            get { return new Uri("/Img/" + ImageFileName, UriKind.Relative); }
        }

        public abstract System.Windows.Media.Brush Status { get; }
    }

    public abstract class LectureCommandBlockModel<TSource, TVideoItem> : CommandsBlockModel<TSource, LectureItem>
    {
        public LectureCommandBlockModel(TSource source, LectureItem item) : base(source, item) { }
        public IEnumerable<TVideoItem> VideoData { get { return Wrap.Subtree().OfType<VideoWrap>().SelectMany(z => z.CommandBlocks.OfType<TVideoItem>()); } }
    }
}
