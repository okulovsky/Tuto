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

namespace Operator
{
    /// <summary>
    /// Interaction logic for InitPage.xaml
    /// </summary>
    public partial class InitWindow : Window
    {
        int InitStage = 0;

        public InitWindow()
        {
            InitializeComponent();
            Log.Start();
            Prompt.Text = "Start camera recording\nMake a funny face and simultaneously press Enter.\nIt is to syncronize camera's video.\nDon't forget to place a sync time from video into a log file later";
            Window.GetWindow(this).KeyDown+=InitKeyDown;
            
            
        }

        void InitKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (InitStage == 0)
            {
                Log.Commit(VideoLib.MontageAction.StartFace);
                InitStage++;
                Prompt.Text = "Start screen's recording and simultaneously press Enter.\nIt is to syncronize screen's video";
                return;
            }
            if (InitStage == 1)
            {
                Log.Commit(VideoLib.MontageAction.StartScreen);
                var main = new MainWindow();
                Application.Current.MainWindow = main;
                Close();
                main.Show();
            }

        }
    }
}
