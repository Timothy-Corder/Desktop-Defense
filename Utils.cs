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
            static int TopFrameThiccness = 30;
            static Brush brush = new SolidBrush(Color.White);
            static Pen pen = new Pen(Color.FromArgb(0x2a,0x2a,0x2a), 1f);
            public FalseWindow(string title, Rectangle bounds, bool visible, Point position)
            {
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
                        if (this.Title == "Desktop Defense Portal")
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            Rectangle topBar = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, TopFrameThiccness);
                            Rectangle portRect = new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness, Bounds.Width, Bounds.Height);
                            e.Graphics.DrawImage(form.Portal, portRect, 0, form.PortalSize * frame, form.PortalSize, form.PortalSize, GraphicsUnit.Pixel);
                            e.Graphics.DrawRectangle(pen, portRect);
                            e.Graphics.FillRectangle(brush, topBar);
                            e.Graphics.DrawRectangle(pen, topBar);
                            RectangleF textBar = new RectangleF(topBar.X, topBar.Y, topBar.Width, topBar.Height);
                            textBar.Offset(topBar.Height / 3, topBar.Height / 3);
                            e.Graphics.DrawString(Title, SystemFonts.DefaultFont, Brushes.Black, textBar);
                            if (form.FrameNum % (form.FPS / form.PortalFPS) == 0)
                            frame++;
                            frame = frame % form.PortalFrames;
                        }
                    }
                }
            }
        }
    }
}
