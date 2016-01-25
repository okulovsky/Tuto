using Editor;
using Editor.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Model;

namespace Tuto.Navigator.Editor
{
    /// <summary>
    /// Interaction logic for EditorPanel.xaml
    /// </summary>
    public partial class EditorPanel : UserControl
    {
        EditorController controller;
        EditorModel model;
        public EditorPanel()
        {
            InitializeComponent();
            DataContextChanged += EditorPanel_DataContextChanged;

			backButton.Click += (s, a) =>
				{
					if (model != null)
						model.WindowState.OnGetBack();
				};

			help.Click += (s, a) =>
				{
					var data = HelpCreator.CreateModeHelp();
					var wnd = new HelpWindow();
					wnd.DataContext = data;
					wnd.Show();
				};

			saveButton.Click += (s, a) =>
				{
					if (model != null) model.Save();
				};

			play.Click += (s, a) =>
				{
					if (model != null) model.WindowState.Paused = false;
				};

			pause.Click += (s, a) =>
				{
					if (model != null) model.WindowState.Paused = true;
				};

			titles.Click += titles_Click;
        }

		void titles_Click(object sender, RoutedEventArgs e)
		{
	        var times = new List<int>();
            var current = 0;
            foreach (var c in model.Montage.Chunks)
            {
                if (c.StartsNewEpisode)
                {
                    times.Add(current);
                    current = 0;
                }
                if (c.Mode == Mode.Face || c.Mode == Mode.Desktop)
                    current += c.Length;
            }
            times.Add(current);
            if (model.Montage.Information.Episodes.Count == 0)
            {
                model.Montage.Information.Episodes.AddRange(Enumerable.Range(0, times.Count).Select(z => new EpisodInfo(Guid.NewGuid())));
            }
            else if (model.Montage.Information.Episodes.Count != times.Count)
            {
                while (model.Montage.Information.Episodes.Count > times.Count)
                    model.Montage.Information.Episodes.RemoveAt(model.Montage.Information.Episodes.Count - 1);
                while (model.Montage.Information.Episodes.Count < times.Count)
                    model.Montage.Information.Episodes.Add(new EpisodInfo(Guid.NewGuid()));
            }

            for (int i = 0; i < times.Count; i++)
            {
                model.Montage.Information.Episodes[i].Duration = TimeSpan.FromMilliseconds(times[i]);
            }


            var wnd = new InfoWindow();
            wnd.DataContext = model.Montage.Information;
            wnd.ShowDialog();
            model.Save();
       
		}

        void EditorPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
			if (controller != null)
			{
				controller.Dispose();
			}
			if (model!=null)
			{
				model.WindowState.UnsubscribeAll(this);
			}
            model = DataContext as EditorModel;
            if (DataContext!=null)
            {
                controller = new EditorController(this.player, model);
				model.WindowState.SubsrcibeByExpression(z => z.Paused, PlayPause);
				PlayPause();
				player.Focus();
            }
        }

		void PlayPause()
		{
			if (model==null) return;
			play.Visibility = model.WindowState.Paused ? Visibility.Visible : Visibility.Hidden;
			pause.Visibility = !model.WindowState.Paused ? Visibility.Visible : Visibility.Hidden;
		}

    }
}
