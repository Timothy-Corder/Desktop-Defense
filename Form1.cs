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
            DragFalseWindow(PointToClient(Cursor.Position));
            foreach (FalseWindow window in FalseWindows)
            {
                window.Draw(e);
            }
        }

        public void DragFalseWindow(Point mousePos)
        {
            foreach (FalseWindow window in FalseWindows)
            {
                if (window.Dragging)
                {
                    window.Bounds = new Rectangle(mousePos.X - window.Difference.X, mousePos.Y - window.Difference.Y, window.Bounds.Width, window.Bounds.Height);
                    if (window.Bounds.X < 0)
                    {
                        window.Bounds = new Rectangle(0, window.Bounds.Y, window.Bounds.Width, window.Bounds.Height);
                    }
                    if (window.Bounds.Y < 0)
                    {
                        window.Bounds = new Rectangle(window.Bounds.X, 0, window.Bounds.Width, window.Bounds.Height);
                    }
                    if (window.Bounds.X + window.Bounds.Width > this.Width)
                    {
                        window.Bounds = new Rectangle(this.Width - window.Bounds.Width, window.Bounds.Y, window.Bounds.Width, window.Bounds.Height);
                    }
                    if (window.Bounds.Y + window.Bounds.Height + FalseWindow.TopFrameThiccness > this.Height)
                    {
                        window.Bounds = new Rectangle(window.Bounds.X, this.Height - window.Bounds.Height - FalseWindow.TopFrameThiccness, window.Bounds.Width, window.Bounds.Height);
                    }
                    return;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            for (int i = FalseWindows.Length - 1; i >= 0; i--)
            {
                FalseWindow window = FalseWindows[i];
                if ((new Rectangle(window.Bounds.X, window.Bounds.Y, window.Bounds.Width, 30)).Contains(e.Location))
                {
                    // Set the offset for dragging
                    window.Difference = new Point(e.Location.X - window.Bounds.X, e.Location.Y - window.Bounds.Y);
                    window.Dragging = true;
                    // Move to the top
                    FalseWindow temp = window;
                    for (int j = 0; j < FalseWindows.Length; j++)
                    {
                        if (FalseWindows[j] == window)
                        {
                            for (int k = j; k < FalseWindows.Length - 1; k++)
                            {
                                FalseWindows[k] = FalseWindows[k + 1];
                                FalseWindows[k].Top = false;
                            }
                            FalseWindows[FalseWindows.Length - 1] = temp;
                            temp.Top = true;
                            break;
                        }
                    }
                    return;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            foreach (FalseWindow window in FalseWindows)
            {
                window.Dragging = false;
            }
        }

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