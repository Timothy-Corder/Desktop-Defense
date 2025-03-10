using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Desktop_Defense
{
    namespace Utils
    {
        public class FalseWindow
        {
            public string Title;
            public Rectangle Bounds;
            public bool Visible;
            public Point Position;
            public WeakReference<Form1> Parent;
            public int frame;
            public static int TopFrameThiccness = 30;
            static Brush brush = new SolidBrush(Color.White);
            static Pen pen = new Pen(Color.FromArgb(0x2a,0x2a,0x2a), 1f);
            public Point Difference;
            public bool Dragging;
            public bool Top = false;
            public event Action<PaintEventArgs> DrawEvent;
            public FalseWindow(string title, Rectangle bounds, bool visible, Point position)
            {
                DrawEvent += Draw;
                Title = title;
                Bounds = bounds;
                Visible = visible;
                Position = position;
                frame = 0;
            }
            public FalseWindow(string title, Rectangle bounds)
            {
                Title = title;
                Bounds = bounds;
                Visible = true;
                Position = Point.Empty;
                frame = 0;
            }
            public FalseWindow(string title, Point position)
            {
                Title = title;
                Bounds = new Rectangle(position.X, position.Y, 800, 600);
                Visible = true;
                Position = position;
                frame = 0;
            }
            public static FalseWindow PortalTemplate()
            {
                return new FalseWindow("Desktop Defense Portal", new Rectangle(0, 0, 200, 200));
            }
            public void Draw(PaintEventArgs e)
            {
                if (Visible)
                {
                    Form1 form;
                    Parent.TryGetTarget(out form);
                    if (form != null)
                    {
                        Rectangle topBar = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, TopFrameThiccness);
                        Rectangle bodyRect = new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness, Bounds.Width, Bounds.Height);
                        switch (this.Title)
                        {
                            case "Desktop Defense Wall":
                                e.Graphics.FillRectangle(Brushes.Gray, bodyRect);
                                break;
                            case "Desktop Defense Tower":
                                e.Graphics.FillRectangle(Brushes.DarkGray, bodyRect);
                                break;
                            case "Desktop Defense Portal":
                                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                e.Graphics.DrawImage(form.Portal, bodyRect, -0.5f, form.PortalSize * frame - 0.5f, form.PortalSize, form.PortalSize, GraphicsUnit.Pixel);
                                if (form.FrameNum % (form.FPS / form.PortalFPS) == 0)
                                    frame++;
                                frame = frame % form.PortalFrames;
                                break;
                        }
                        e.Graphics.DrawRectangle((this.Top) ? pen : Pens.DarkGray, bodyRect);
                        e.Graphics.FillRectangle(Brushes.White, topBar);
                        e.Graphics.DrawRectangle((this.Top) ? pen : Pens.DarkGray, topBar);
                        RectangleF textBar = new RectangleF(topBar.X, topBar.Y, topBar.Width, topBar.Height);
                        textBar.Offset(topBar.Height / 3, topBar.Height / 3);
                        e.Graphics.DrawString(Title, SystemFonts.DefaultFont, (this.Top)? Brushes.Black : Brushes.Gray, textBar);
                    }
                }
            }
        }
    }
}
