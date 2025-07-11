using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Defense
{
    public class FalseElement
    {
        public Rectangle Bounds;
        public bool Visible;
        public int State;
        public Image Sprite;
        public Action Callback;
        private WeakReference<FalseWindow> _parentRef;
        public FalseWindow Parent
        {
            get
            {
                _parentRef.TryGetTarget(out FalseWindow fWnd);
                return fWnd;
            }
            set
            {
                _parentRef = new WeakReference<FalseWindow>(value);
            }
        }
        public bool StaticSprite = true;
        public int FrameCount = 1;
        private int _frame = 0;
        public int Frame
        {
            get => _frame;
            set
            {
                _frame = value % FrameCount;
            }
        }
        public int FPS;
        public bool OnFrame
        {
            get
            {
                if (StaticSprite)
                {
                    return true;
                }
                if (FPS > 0 && Parent != null && Parent.Parent != null)
                {
                    if (Parent.Parent.FrameNum % (Parent.Parent.FPS / FPS) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public Size spriteSize;
        public bool Draggable;

        // Dragging fields
        private bool _dragging = false;
        private Point _dragOffset;

        public FalseElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent)
        {
            Bounds = bounds;
            Sprite = sprite;
            Callback = callback;
            State = 0;
            Visible = true;
            Parent = parent;
            Draggable = false;
        }
        public FalseElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent, bool draggable)
        {
            Bounds = bounds;
            Sprite = sprite;
            Callback = callback;
            State = 0;
            Visible = true;
            Parent = parent;
            Draggable = draggable;
        }

        public FalseElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent, int frameCount)
        {
            Bounds = bounds;
            Sprite = sprite;
            Callback = callback;
            State = 0;
            Visible = true;
            Parent = parent;
            StaticSprite = false;
            FrameCount = frameCount;
            Draggable = false;
        }
        public FalseElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent, int frameCount, bool draggable)
        {
            Bounds = bounds;
            Sprite = sprite;
            Callback = callback;
            State = 0;
            Visible = true;
            Parent = parent;
            StaticSprite = false;
            FrameCount = frameCount;
            Draggable = draggable;
        }

        public virtual void Draw(PaintEventArgs e)
        {
            if (Parent != null && Visible)
            {
                // Constrain within parent bounds
                if (Bounds.X < Parent.Bounds.X)
                {
                    Bounds.X = Parent.Bounds.X;
                }
                if (Bounds.Y < Parent.Bounds.Y + FalseWindow.TopFrameThiccness)
                {
                    Bounds.Y = Parent.Bounds.Y + FalseWindow.TopFrameThiccness;
                }
                if (Bounds.X + Bounds.Width > Parent.Bounds.X + Parent.Bounds.Width)
                {
                    Bounds.X = Parent.Bounds.X + Parent.Bounds.Width - Bounds.Width;
                }
                if (Bounds.Y + Bounds.Height > Parent.Bounds.Y + Parent.Bounds.Height + FalseWindow.TopFrameThiccness)
                {
                    Bounds.Y = Parent.Bounds.Y + Parent.Bounds.Height - Bounds.Height + FalseWindow.TopFrameThiccness;
                }

                // Draw element
                e.Graphics.DrawImage(Sprite, Bounds, 0, (Sprite.Height / FrameCount) * State, Sprite.Width, Sprite.Height / FrameCount, GraphicsUnit.Pixel);
            }
        }

        public void Trigger()
        {
            Callback.Invoke();
        }

        // Handle mouse events
        public virtual void MouseDown(MouseEventArgs e)
        {
            Point mousePos = Parent.Parent.PointToClient(Cursor.Position);
            if ((Bounds.Contains(mousePos) && Draggable) || Parent.Dragging)
            {
                _dragging = true;
                _dragOffset = new Point(mousePos.X - Bounds.X, mousePos.Y - Bounds.Y);
            }
        }

        public virtual void MouseMove(MouseEventArgs e)
        {
            Point mousePos = Parent.Parent.PointToClient(Cursor.Position);
            if (_dragging || Parent.Dragging)
            {
                Bounds = new Rectangle(mousePos.X - _dragOffset.X, mousePos.Y - _dragOffset.Y, Bounds.Width, Bounds.Height);
            }
        }

        public virtual void MouseUp(MouseEventArgs e)
        {
            _dragging = false;
        }
    }
}
