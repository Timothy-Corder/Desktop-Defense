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
        private int flashAlpha = -100;
        internal int FrameNum = 0;
        internal int FPS = 60;
        internal int PortalFPS = 10;
        internal ImgButton CloseButton;
        internal Grass Grass = new Grass();
        private int _minGroundSize;
        public Form1(int portalCount, int wallCount, int towerCount)
        {
            _minGroundSize = Grass.sprites[0][0].Width * 3 * Grass.ScaleFactor;
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
            Bitmap clsBtn = Resources.CloseButton;
            this.CloseButton = new ImgButton(new Rectangle(Screen.FromControl(this).WorkingArea.Width - (clsBtn.Width * 4), 0, clsBtn.Width * 4, (clsBtn.Height * 4) / 3), clsBtn, this.Close, new WeakReference<Form1>(this));

            for (int i = 0; i < portalCount; i++)
            {
                FalseWindows[i] = FalseWindow.PortalTemplate();
                FalseWindows[i].Parent = new WeakReference<Form1>(this);
            }
            for (int i = portalCount; i < portalCount + wallCount; i++)
            {
                FalseWindows[i] = new FalseWindow("Desktop Defense Ground", new Rectangle(200 + (i * 10), 200 + (i * 10), 150, 150), true, true);
                FalseWindows[i].Parent = new WeakReference<Form1>(this);
            }
            for (int i = portalCount + wallCount; i < portalCount + wallCount + towerCount; i++)
            {
                FalseWindows[i] = new FalseWindow("Desktop Defense Tower", new Point(0, 0));
                FalseWindows[i].Parent = new WeakReference<Form1>(this);
            }
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
                flashAlpha += 5;
                if (flashAlpha >= 500)
                {
                    flashAlpha = 0;
                    flashing = false;
                }
                return;
            }
            DragFalseWindow(PointToClient(Cursor.Position));
            ScaleFalseWindow(PointToClient(Cursor.Position));
            foreach (FalseWindow window in FalseWindows)
            {
                window.Draw(e);
            }
            for (int i = FalseWindows.Length - 1; i >= 0; i--)
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
            for (int i = FalseWindows.Length - 1; i >= 0; i--)
            {
                FalseWindow window = FalseWindows[i];

                // Check if the mouse is on the resize handle
                if (window.Resizeable && new Rectangle(window.Bounds.X + window.Bounds.Width - 5, window.Bounds.Y + FalseWindow.TopFrameThiccness + window.Bounds.Height - 5, 10, 10).Contains(PointToClient(MousePosition)))
                {
                    window.Resizing = true;
                }

                // Check if the mouse is in the title bar
                if ((new Rectangle(window.Bounds.X, window.Bounds.Y, window.Bounds.Width, 30)).Contains(e.Location))
                {
                    // Set the offset for dragging
                    window.Difference = new Point(e.Location.X - window.Bounds.X, e.Location.Y - window.Bounds.Y);
                    window.Dragging = true;
                }
                if (window.Dragging || window.Resizing)
                {
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
                else
                {
                    window.Top = false;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            CloseButton.ClickUp(e.Location);
            foreach (FalseWindow window in FalseWindows)
            {
                window.Dragging = false;
                window.Resizing = false;
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