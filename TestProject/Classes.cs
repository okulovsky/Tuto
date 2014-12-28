using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TestProject
{
    public class Item {

        public List<Item> Items { get; set; }
        public Item()
        {
            Items = new List<Item>();
        }
    }

    public class Item<T> : Item
    {
        T Data { get; set; }
    }

    public interface ILecture
    {

    }

    public class LectureItem<T> : Item<T>, ILecture
    {
        public string LectureName { get; set; }
    }

    public interface IVideo
    {
    }

    public class VideoItem<T> : Item<T>, IVideo
    {
        public string VideoName { get; set; }
    }


    class Program
    {
        [STAThread]
        public static void Main()
        {
            var wnd = new MainWindow();
            wnd.DataContext = new LectureItem<string> { Items = { new LectureItem<string>(), new VideoItem<string>() } };
            new Application().Run(wnd);
        }
    }
}
