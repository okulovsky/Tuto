using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoLib;
using System.Windows.Media.Animation;
using System.IO;

namespace Operator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer clockTimer;

        public MainWindow()
        {
            InitializeComponent();
         
            Status.Opacity = 0;
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Tick += new EventHandler(TimerTick);
           
            clockTimer.Start();
            KeyDown += new KeyEventHandler(MainWindowKeyDown);

            var documentPath = "help.rtf";

            var fileStream = File.Open(documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            TextRange textRange = new TextRange(Sufler.ContentStart, Sufler.ContentEnd);
            textRange.Load(fileStream, DataFormats.Rtf);
        }

       
        

       
        void ShowStatus(string statusSource)
        {
            Status.SetResourceReference(Image.SourceProperty, statusSource);
            ((Storyboard)FindResource("statusFadeout")).Begin(Status);
        }

        void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            MontageAction action = MontageAction.Commit;


            switch (e.Key)
            {
                case Key.Enter: action = MontageAction.Commit; break;
                case Key.Decimal: action = MontageAction.Delete; break;
                case Key.NumPad1: action = MontageAction.Screen; break;
                case Key.NumPad2: action = MontageAction.Face; break;
                case Key.Add: action = MontageAction.CommitAndSplit; break;
                case Key.NumPad9:
                    this.Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset - 100);
                    return;
                case Key.NumPad6:
                    this.Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + 100);
                    return;
                case Key.NumPad8:
                    this.Viewer.Zoom += 20;
                    return;
                case Key.NumPad5:
                    this.Viewer.Zoom -= 20;
                    return;

                default:
                    ShowStatus("question");
                    return;
            }


            
            Log.Commit(action);

            if (action != MontageAction.Delete)
                ShowStatus("clapper");
            else
                ShowStatus("trash");


            if (action == MontageAction.Face)
                VideoSource.SetResourceReference(Image.SourceProperty, "face");
            if (action == MontageAction.Screen)
                VideoSource.SetResourceReference(Image.SourceProperty, "screen");

            // refresh displayed clock label now
            TimerTick(null, EventArgs.Empty);
            clockTimer.Stop();
            clockTimer.Start();


        }

  
        void TimerTick(object sender, EventArgs e)
        {
            var interval=Log.TimeFromLastCommit;
            ClockFromLastCommit.Content = string.Format("{0:D2}:{1:D2}", interval.Minutes, interval.Seconds);

            var fromSplit = Log.TimeFromLastSplit;
            ClockGoodInCurrentPart.Content = string.Format("{0:D2}:{1:D2}", fromSplit.Minutes, fromSplit.Seconds);

            var fromStart = Log.TimeFromStart;
            ClockGoodTotal.Content = string.Format("{0:D2}:{1:D2}", fromStart.Minutes, fromStart.Seconds);

        }
    }
}
