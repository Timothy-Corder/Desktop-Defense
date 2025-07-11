using Desktop_Defense.Utils;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Timer = System.Windows.Forms.Timer;

namespace Desktop_Defense
{
    public partial class GameForm : Form
    {
        private Timer refreshTimer;
        internal Image Portal;
        internal int PortalSize;
        internal int PortalFrames;
        internal List<FalseWindow> FalseWindows;
        public Image BackgroundImage;
        private Image Title;
        private bool flashing = true;
        private int flashAlpha = -100;
        internal int FrameNum = 0;
        internal int FPS = 60;
        internal int PortalFPS = 10;
        internal ImgButton CloseButton;
        internal Grass Grass = new Grass();
        internal ManaSprite ManaSprite = new ManaSprite();
        private int _minGroundSize;
        internal Image Anchor;
        internal int AnchorSize;
        internal int AnchorFrames;
        internal int AnchorFPS = 30;


        public GameForm(int portalCount, int wallCount, int towerCount)
        {
            _minGroundSize = (int)(Grass.sprites[0][0].Width * 2 * Grass.ScaleFactor * 1.25);
            this.Text = "Desktop Defense";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true; // reduce flicker
            this.ResizeBegin += (o, e) => { this.OnResizeEnd(e); };
            this.Portal = Resources.Portal;
            this.PortalSize = Portal.Width;
            this.PortalFrames = Portal.Height / PortalSize;
            this.Title = Resources.Title;
            this.FalseWindows = new List<FalseWindow>();
            Bitmap clsBtn = Resources.CloseButton;
            this.CloseButton = new ImgButton(new Rectangle(Screen.FromControl(this).WorkingArea.Width - (clsBtn.Width * 4), 0, clsBtn.Width * 4, (clsBtn.Height * 4) / 3), clsBtn, this.Close, new WeakReference<GameForm>(this));

            for (int i = 0; i < portalCount; i++)
            {
                FalseWindows.Add(new PortalWindow(this));
            }
            for (int i = portalCount; i < portalCount + wallCount; i++)
            {
                FalseWindows.Add(new GroundWindow(i, this));
            }
            for (int i = portalCount + wallCount; i < portalCount + wallCount + towerCount; i++)
            {
                FalseWindows.Add(new FalseWindow("Desktop Defense Tower", new Point(0, 0)));
                FalseWindows[i].Parent = this;
            }
            FalseWindows.Add(new HotbarWindow(this));
            this.Move += newScreenshot;

            // Refresh the drawing periodically
            refreshTimer = new Timer();
            refreshTimer.Interval = 1000/FPS; // 60fps
            refreshTimer.Tick += (s, e) => this.Invalidate();
            refreshTimer.Start();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetProcessDPIAware();
        }
        internal void newScreenshot(object? o, EventArgs e)
        {
            Screenshotter Screenshotter = new Screenshotter(this);
            Image newScreenie = Screenshotter.TakeScreenshot(Screen.FromControl(this));
            if (newScreenie != null)
            {
                BackgroundImage = newScreenie;
            }
            Debug.WriteLine($"{Screen.FromControl(this).WorkingArea.Width}, {Screen.FromControl(this).WorkingArea.Height}");
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(e);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(BackgroundImage, 0, 0, this.Width, this.Height);
            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            FrameNum = (FrameNum+1) % 60;
            
            if (flashing)
            {
                
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(Math.Clamp(flashAlpha,0,255), Color.White)), 0, 0, this.Width, this.Height);
                e.Graphics.DrawImage(Resources.Title, this.Width / 2 - (Resources.Title.Width * 10) / 2, this.Height / 2 - (Resources.Title.Height * 10) / 2, (Resources.Title.Width * 10), (Resources.Title.Height * 10));
                flashAlpha += 2;
                if (flashAlpha >= 500)
                {
                    flashAlpha = 0;
                    flashing = false;
                }
                return;
            }
            //DragFalseWindow(PointToClient(Cursor.Position));
            //ScaleFalseWindow(PointToClient(Cursor.Position));
            foreach (FalseWindow window in FalseWindows)
            {
                window.Draw(e);
            }
            for (int i = FalseWindows.Count - 1; i >= 0; i--)
            {
                FalseWindow window = FalseWindows[i];
                if (window.Resizeable && new Rectangle(window.Bounds.X + window.Bounds.Width - 5, window.Bounds.Y + FalseWindow.TopFrameThiccness + window.Bounds.Height - 5, 10, 10).Contains(PointToClient(MousePosition)))
                {
                    Cursor = Cursors.SizeNWSE;
                    break;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            CloseButton.Hover(PointToClient(MousePosition));
            CloseButton.Draw(e);
        }

        //public void DragFalseWindow(Point mousePos)
        //{
        //    foreach (FalseWindow window in FalseWindows)
        //    {
        //        if (window.Dragging)
        //        {
        //            window.Bounds = new Rectangle(mousePos.X - window.Difference.X, mousePos.Y - window.Difference.Y, window.Bounds.Width, window.Bounds.Height);
        //            if (window.Bounds.X < 0)
        //            {
        //                window.Bounds = new Rectangle(0, window.Bounds.Y, window.Bounds.Width, window.Bounds.Height);
        //            }
        //            if (window.Bounds.Y < 0)
        //            {
        //                window.Bounds = new Rectangle(window.Bounds.X, 0, window.Bounds.Width, window.Bounds.Height);
        //            }
        //            if (window.Bounds.X + window.Bounds.Width > this.Width)
        //            {
        //                window.Bounds = new Rectangle(this.Width - window.Bounds.Width, window.Bounds.Y, window.Bounds.Width, window.Bounds.Height);
        //            }
        //            if (window.Bounds.Y + window.Bounds.Height + FalseWindow.TopFrameThiccness > this.Height)
        //            {
        //                window.Bounds = new Rectangle(window.Bounds.X, this.Height - window.Bounds.Height - FalseWindow.TopFrameThiccness, window.Bounds.Width, window.Bounds.Height);
        //            }
        //            return;
        //        }
        //    }
        //}
        public void ScaleFalseWindow(Point mousePos)
        {
            foreach (FalseWindow window in FalseWindows)
            {
                if (window.Resizeable && window.Resizing)
                {
                    window.Bounds = new Rectangle(window.Bounds.X, window.Bounds.Y, mousePos.X - window.Bounds.X, mousePos.Y - window.Bounds.Y - FalseWindow.TopFrameThiccness);

                    window.Bounds.Width = Math.Max(window.Bounds.Width, _minGroundSize);
                    window.Bounds.Height = Math.Max(window.Bounds.Height, _minGroundSize);
                    return;
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            CloseButton.ClickDown(e.Location);
            // Only send the mouse down to the topmost visible window under the mouse
            bool windowUnder = false;
            for (int i = FalseWindows.Count - 1; i >= 0; i--) // reverse order = topmost first
            {
                var wnd = FalseWindows[i];
                if (wnd.Visible && wnd.Bounds.Contains(e.Location))
                {
                    wnd.MouseDown(e);
                    windowUnder = true;
                    break; // Only let one window handle it
                }
            }
            if (!windowUnder)
            {
                foreach(FalseWindow window in FalseWindows)
                {
                    window.Top = false;
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            foreach (FalseWindow window in FalseWindows)
            {
                window.MouseMove(e);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            CloseButton.ClickUp(e.Location);
            foreach (FalseWindow window in FalseWindows)
            {
                window.MouseUp(e);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

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