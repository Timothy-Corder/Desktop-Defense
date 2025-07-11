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
        private WeakReference<GameForm> _parentRef;
        public string Title;
        public Rectangle Bounds;
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
            if (Resizeable && new Rectangle(Bounds.X + Bounds.Width - 5, Bounds.Y + FalseWindow.TopFrameThiccness + Bounds.Height - 5, 10, 10).Contains(Parent.PointToClient(e.Location)))
            {
                Resizing = true;
            }

            // Check if the mouse is in the title bar
            if ((new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 30)).Contains(e.Location))
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
                    Bounds = new Rectangle(Bounds.X, Bounds.Y, mousePos.X - Bounds.X, mousePos.Y - Bounds.Y);
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
