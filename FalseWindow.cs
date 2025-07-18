using Desktop_Defense.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Defense
{
    public class FalseWindow
    {
        enum ResizeCorner
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private WeakReference<GameForm> _parentRef;
        public string Title;
        public Rectangle Bounds;
        public Rectangle FullBounds
        {
            get
            {
                return new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + TopFrameThiccness);
            }
        }
        public Rectangle TopBarBounds
        {
            get
            {
                return new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, TopFrameThiccness);
            }
        }
        public bool Visible;
        public bool Animating;
        public GameForm Parent
        {
            get
            {
                GameForm form;
                _parentRef.TryGetTarget(out form);
                return form;
            }
            set
            {
                _parentRef = new WeakReference<GameForm>(value);
            }
        }
        public int frame;
        public static int TopFrameThiccness = 30;
        static Brush brush = new SolidBrush(Color.White);
        static Pen pen = new Pen(Color.FromArgb(0x2a, 0x2a, 0x2a), 1f);
        public Point Difference;
        public bool Dragging;
        public bool Resizeable;
        public bool Resizing;
        public bool Top = false;
        public List<FalseElement> Elements = new List<FalseElement>();
        public event Action<PaintEventArgs> DrawEvent;
        ResizeCorner activeResizeCorner = ResizeCorner.None;


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
        public virtual void Draw(PaintEventArgs e)
        {
            if (Visible)
            {
                if (Parent != null)
                {
                    Rectangle topBar = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, TopFrameThiccness);
                    Rectangle bodyRect = new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness, Bounds.Width, Bounds.Height);
                    e.Graphics.DrawRectangle((this.Top) ? pen : Pens.Gray, bodyRect);
                    e.Graphics.FillRectangle(Brushes.White, topBar);
                    e.Graphics.DrawRectangle((this.Top) ? pen : Pens.Gray, topBar);
                    RectangleF textBar = new RectangleF(topBar.X, topBar.Y, topBar.Width, topBar.Height);
                    textBar.Offset(topBar.Height / 3, topBar.Height / 3);
                    e.Graphics.DrawString(Title, SystemFonts.DefaultFont, (this.Top) ? Brushes.Black : Brushes.Gray, textBar);
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        if (Elements[i] != null)
                        {
                            Elements[i].Draw(e);
                        }
                    }
                }
            }
        }
        public virtual void MouseDown(MouseEventArgs e)
        {
            Top = true;
            if (Resizeable)
            {
                Point mousePos = Parent.PointToClient(e.Location);
                Rectangle bounds = Bounds;

                Rectangle bottomRight = new Rectangle(bounds.X + bounds.Width - 10, bounds.Y + FalseWindow.TopFrameThiccness + bounds.Height - 10, 10, 10);
                Rectangle bottomLeft = new Rectangle(bounds.X, bounds.Y + FalseWindow.TopFrameThiccness + bounds.Height - 10, 10, 10);
                Rectangle topRight = new Rectangle(bounds.X + bounds.Width - 10, bounds.Y, 10, 10);
                Rectangle topLeft = new Rectangle(bounds.X, bounds.Y, 10, 10);

                if (bottomRight.Contains(mousePos))
                {
                    Resizing = true;
                    activeResizeCorner = ResizeCorner.BottomRight;
                }
                else if (bottomLeft.Contains(mousePos))
                {
                    Resizing = true;
                    activeResizeCorner = ResizeCorner.BottomLeft;
                }
                else if (topRight.Contains(mousePos))
                {
                    Resizing = true;
                    activeResizeCorner = ResizeCorner.TopRight;
                }
                else if (topLeft.Contains(mousePos))
                {
                    Resizing = true;
                    activeResizeCorner = ResizeCorner.TopLeft;
                }
                else
                {
                    Resizing = false;
                    activeResizeCorner = ResizeCorner.None;
                }
            }

            // Check if the mouse is in the title bar
            if (TopBarBounds.Contains(e.Location) && !Resizing)
            {
                // Set the offset for dragging
                Difference = new Point(e.Location.X - Bounds.X, e.Location.Y - Bounds.Y);
                Dragging = true;
            }
            var fWnds = Parent.FalseWindows;
            // Move to the top
            for (int i = 0; i < fWnds.Count; i++)
            {
                if (fWnds[i] == this)
                {
                    for (int k = i; k < fWnds.Count - 1; k++)
                    {
                        fWnds[k] = fWnds[k + 1];
                        fWnds[k].Top = false;
                    }
                    fWnds[fWnds.Count - 1] = this;
                    Top = true;
                    break;
                }
            }
            foreach (FalseElement element in Elements)
            {
                element.MouseDown(e);
            }
        }
        public virtual void MouseMove(MouseEventArgs e)
        {
            if (Visible && Parent != null)
            {
                Point mousePos = Parent.PointToClient(Cursor.Position);
                if (Dragging)
                {
                    Bounds = new Rectangle(mousePos.X - Difference.X, mousePos.Y - Difference.Y, Bounds.Width, Bounds.Height);
                }
                if (Resizeable && Resizing)
                {
                    Rectangle newBounds = Bounds;

                    switch (activeResizeCorner)
                    {
                        case ResizeCorner.BottomRight:
                            newBounds.Width = mousePos.X - Bounds.X;
                            newBounds.Height = mousePos.Y - Bounds.Y - TopFrameThiccness;
                            break;

                        case ResizeCorner.BottomLeft:
                            int newRightX = Bounds.Right;
                            newBounds.X = mousePos.X;
                            newBounds.Width = newRightX - newBounds.X;
                            newBounds.Height = mousePos.Y - Bounds.Y - TopFrameThiccness;
                            break;

                        case ResizeCorner.TopRight:
                            int newBottomY = Bounds.Bottom;
                            newBounds.Y = mousePos.Y;
                            newBounds.Height = newBottomY - newBounds.Y;
                            newBounds.Width = mousePos.X - Bounds.X;
                            break;

                        case ResizeCorner.TopLeft:
                            newRightX = Bounds.Right;
                            newBottomY = Bounds.Bottom;
                            newBounds.X = mousePos.X;
                            newBounds.Y = mousePos.Y;
                            newBounds.Width = newRightX - newBounds.X;
                            newBounds.Height = newBottomY - newBounds.Y;
                            break;
                    }

                    // Clamp minimum dimensions
                    if (newBounds.Width < 150)
                    {
                        if (activeResizeCorner == ResizeCorner.BottomLeft || activeResizeCorner == ResizeCorner.TopLeft)
                            newBounds.X -= (150 - newBounds.Width);
                        newBounds.Width = 150;
                    }

                    if (newBounds.Height < 150)
                    {
                        if (activeResizeCorner == ResizeCorner.TopLeft || activeResizeCorner == ResizeCorner.TopRight)
                            newBounds.Y -= (150 - newBounds.Height);
                        newBounds.Height = 150;
                    }

                    Bounds = newBounds;
                }

                foreach (FalseElement element in Elements)
                {
                    if (element != null && element.Visible)
                    {
                        element.MouseMove(e);
                    }
                }
            }
        }
        public virtual void MouseUp(MouseEventArgs e)
        {
            if (Visible && Parent != null)
            {
                Dragging = false;
                Resizing = false;
            }
            foreach (FalseElement element in Elements)
            {
                if (element != null && element.Visible)
                {
                    element.MouseUp(e);
                }
            }
        }
        public void AddElement(FalseElement element)
        {
            if (element != null)
            {
                element.Parent = this;
                Elements.Add(element);
            }
        }
        public void RemoveElement(FalseElement element)
        {
            if (element != null)
            {
                Elements.Remove(element);
            }
        }
    }
}
