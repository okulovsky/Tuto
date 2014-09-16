using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tuto.Model;
using Tuto.TutoServices;

namespace Tester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var globalData = new GlobalData();
            globalData.TitleImage = "logo.png";
            globalData.TopicsRoot.Items.Add(
                new Topic
                {
                    Caption = "Основы программирования",
                    Items = {
                    new Topic { Caption="Введение в C#", Items = {
                        new Topic { Caption="Hello, world!", Items = {
                            new Topic { Caption="Методы" }}}}}}
                });
            var paris = new BasicProgrammingComposer();
            var bmp = paris.Compose(globalData, new string[] {
                "Основы программирования",
                "Введение в C#",
                "Hello, world!",
                "Методы"});

            var box = new PictureBox();
            box.Image = bmp;
            box.Dock = DockStyle.Fill;
            var wnd = new Form();
            wnd.WindowState = FormWindowState.Maximized;
            wnd.Controls.Add(box);
            Application.Run(wnd);

        }
    }
}
