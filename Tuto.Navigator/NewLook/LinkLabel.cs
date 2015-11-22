using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Tuto.Navigator.NewLook
{
    public class LinkLabel : Label
    {
        public string Url { get; set; }
        public LinkLabel()
        {
            this.MouseDoubleClick += (s, a) =>
            { 
                if (Url != null) 
                    Process.Start(Url);
            };
        }

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(LinkLabel));
 
    }
}
