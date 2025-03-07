using Desktop_Defense.Utils;
using System.Runtime.InteropServices;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace Desktop_Defense
{
    public partial class Form1 : Form
    {
        private Timer refreshTimer;
        internal Image Portal;
        internal int PortalSize;
        internal int PortalFrames;
        internal FalseWindow[] FalseWindows;
        public Image BackgroundImage;
        private Image Title;
        private bool flashing = true;
        private int flashAlpha = 0;
        internal int FrameNum = 0;
        internal int FPS = 60;
        internal int PortalFPS = 10;
        public Form1(int portalCount, int wallCount, int towerCount)
        {
            this.Text = "Desktop Defense";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true; // reduce flicker
            this.ResizeBegin += (o, e) => { this.OnResizeEnd(e); };
            this.Portal = Resources.Portal;
            this.PortalSize = Portal.Width;
            this.PortalFrames = Portal.Height / PortalSize;
            this.Title = Resources.Title;
            this.FalseWindows = new FalseWindow[portalCount + wallCount + towerCount];

            for (int i = 0; i < portalCount; i++)
            {
                FalseWindows[i] = FalseWindow.PortalTemplate();
                FalseWindows[i].Parent = new WeakReference<Form1>(this);
            }
            for (int i = portalCount; i < portalCount + wallCount; i++)
            {
                FalseWindows[i] = new FalseWindow("Desktop Defense Wall", new Point(0, 0));
                FalseWindows[i].Parent = new WeakReference<Form1>(this);
            }
            for (int i = portalCount + wallCount; i < portalCount + wallCount + towerCount; i++)
            {
                FalseWindows[i] = new FalseWindow("Desktop Defense Tower", new Point(0, 0));
                FalseWindows[i].Parent = new WeakReference<Form1>(this);
            }
            // Refresh the drawing periodically
            refreshTimer = new Timer();
            refreshTimer.Interval = 1000/FPS; // 60fps
            refreshTimer.Tick += (s, e) => this.Invalidate();
            refreshTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(BackgroundImage, 0, 0, this.Width, this.Height);
            FrameNum = (FrameNum+1) % 60;
            
            if (flashing)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(Math.Min(flashAlpha,255), Color.White)), 0, 0, this.Width, this.Height);
                //e.Graphics.DrawImage(Resources.Title, this.Width / 2 - Resources.Title.Width, this.Height / 2 - 300, 800, 600);
                flashAlpha += 10;
                if (flashAlpha >= 500)
                {
                    flashAlpha = 0;
                    flashing = false;
                }
                return;
            }

            foreach (FalseWindow window in FalseWindows)
            {
                window.Draw(e);
            }
        }
        // P/Invoke declarations

        // Delegate for EnumWindows callback.
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}