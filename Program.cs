using System.Drawing.Imaging;
using System.Drawing;
using System;
using static TimUtils.Functions;
using static TimUtils.TimOperator;
using System.Diagnostics;

namespace Desktop_Defense
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();
            //MakeWalls(3).Wait();
            GameForm app = new GameForm(1, 5 , 0);

            app.Show();

            app.Disposed += (sender, e) => { Application.Exit(); };
            Bitmap captureBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            Rectangle captureRectangle = Screen.PrimaryScreen.Bounds;
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            app.BackgroundImage = captureBitmap;

            Application.Run();
        }
    }
}
