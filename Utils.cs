using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using TimUtils;

namespace Desktop_Defense
{
    namespace Utils
    {
        public struct IntVector
        {
            private Vector2 _value;
            public Vector2 Value
            {
                get => _value.Intify();
                set => _value = value.Intify();
            }
            public IntVector(Vector2 v)
            {
                _value = v.Intify();
            }
            public static implicit operator Vector2(IntVector v)
            {
                return v.Value;
            }
            public static implicit operator IntVector(Vector2 v)
            {
                return new IntVector(v);
            }
        }
        public class ImgButton
        {
            public Rectangle Bounds;
            public bool Visible;
            public int State;
            public Bitmap Sprite;
            public Action Callback;
            public WeakReference<Form1> Parent;
            public ImgButton(Rectangle bounds, Bitmap sprite, Action callback, WeakReference<Form1> parent)
            {
                Bounds = bounds;
                Sprite = sprite;
                Callback = callback;
                State = 0;
                Visible = true;
                Parent = parent;
            }
            public void Draw(PaintEventArgs e)
            {
                Form1 form;
                Parent.TryGetTarget(out form);
                if (form != null)
                {
                    if (Visible)
                    {
                        e.Graphics.DrawImage(Sprite, Bounds, 0, ((Sprite.Height - 0.5f)/3) * State, (Sprite.Width - 0.5f), (Sprite.Height - 0.5f) / 3, GraphicsUnit.Pixel);
                    }
                }
            }
            public void Hover(Point mousePos)
            {
                if (State != 2)
                {
                    if (Bounds.Contains(mousePos))
                    {
                        State = 1;
                    }
                    else
                    {
                        State = 0;
                    }
                }
            }
            public void ClickDown(Point mousePos)
            {
                if (Bounds.Contains(mousePos))
                {
                    State = 2;
                }
            }
            public void ClickUp(Point mousePos)
            {
                if (Bounds.Contains(mousePos))
                {
                    Callback.Invoke();
                }
                else
                {
                    State = 0;
                }
            }
        }
        public class FalseWindow
        {
            public string Title;
            public Rectangle Bounds;
            public bool Visible;
            public WeakReference<Form1> Parent;
            public int frame;
            public static int TopFrameThiccness = 30;
            static Brush brush = new SolidBrush(Color.White);
            static Pen pen = new Pen(Color.FromArgb(0x2a,0x2a,0x2a), 1f);
            public Point Difference;
            public bool Dragging;
            public bool Resizeable;
            public bool Resizing;
            public bool Top = false;
            public event Action<PaintEventArgs> DrawEvent;
            public FalseWindow(string title, Rectangle bounds, bool visible, bool resizeable)
            {
                DrawEvent += Draw;
                Title = title;
                Bounds = bounds;
                Visible = visible;
                frame = 0;
                Resizeable = resizeable;
            }
            public FalseWindow(string title, Rectangle bounds)
            {
                Title = title;
                Bounds = bounds;
                Visible = true;
                frame = 0;
            }
            public FalseWindow(string title, Point position)
            {
                Title = title;
                Bounds = new Rectangle(position.X, position.Y, 800, 600);
                Visible = true;
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
