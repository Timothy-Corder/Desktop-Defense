using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using TimUtils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Desktop_Defense
{
    namespace Utils
    {
        public class ManaSprite
        {
            public Bitmap[] sprites;
            public const int ScaleFactor = 4;
            public ManaSprite()
            {
                int spriteCount;
                int size = Resources.ManaBall.Width;
                spriteCount = Resources.ManaBall.Height / Resources.ManaBall.Width;
                sprites = new Bitmap[spriteCount];
                for (int i = 0; i < spriteCount; i++)
                {
                    Bitmap bmp = new Bitmap(size, size);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(Resources.ManaBall, new RectangleF(0, 0, size, size), new Rectangle(i * size, 0, size, size), GraphicsUnit.Pixel);
                    sprites[i] = bmp;
                }
            }
        }
        public class Grass
        {
            public Bitmap[][] sprites;
            const int size = 16;
            public const int ScaleFactor = 4;
            public Grass()
            {
                sprites = new Bitmap[3][];
                for (int i = 0; i < 3; i++)
                {
                    sprites[i] = new Bitmap[3];
                    for (int j = 0; j < 3; j++)
                    {
                        Bitmap bmp = new Bitmap(size, size);
                        Graphics g = Graphics.FromImage(bmp);
                        g.DrawImage(Resources.Grass, new RectangleF(0, 0, size, size), new Rectangle(j * size, i * size, size, size), GraphicsUnit.Pixel);
                        sprites[i][j] = bmp;
                    }
                }
            }
        }
        public class Screenshotter
        {
            public WeakReference<Form1> Parent;
            public Screenshotter(Form1 p)
            {
                Parent = new WeakReference<Form1>(p);
            }
            public Bitmap? TakeScreenshot(Screen screen)
            {
                if (Parent != null)
                {
                    Form1 form;
                    Parent.TryGetTarget(out form);
                    if (form != null)
                    {
                        form.Hide();
                        Bitmap captureBitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
                        Rectangle captureRectangle = screen.Bounds;
                        Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                        captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                        form.Show();
                        return captureBitmap;
                    }
                }
                return null;
            }
        }
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
                        e.Graphics.DrawImage(Sprite, Bounds, 0, (Sprite.Height / 3) * State, Sprite.Width, Sprite.Height / 3, GraphicsUnit.Pixel);
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
            private WeakReference<Form1> _parentRef;
            public string Title;
            public Rectangle Bounds;
            public bool Visible;
            public bool Animating;
            public Form1 Parent
            {
                get
                {
                    Form1 form;
                    _parentRef.TryGetTarget(out form);
                    return form;
                }
                set
                {
                    _parentRef = new WeakReference<Form1>(value);
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
            public List<FWndElement> Elements = new List<FWndElement>();
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
                if (Dragging || Resizing)
                {
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
                    return;
                }
                else
                {
                    Top = false;
                }
                foreach (FWndElement element in Elements)
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
                    foreach (FWndElement element in Elements)
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
                foreach (FWndElement element in Elements)
                {
                    if (element != null && element.Visible)
                    {
                        element.MouseUp(e);
                    }
                }
            }
            public void AddElement(FWndElement element)
            {
                if (element != null)
                {
                    element.Parent = this;
                    Elements.Add(element);
                }
            }
            public void RemoveElement(FWndElement element)
            {
                if (element != null)
                {
                    Elements.Remove(element);
                }
            }
        }
        public class PortalWindow : FalseWindow
        {
            public PortalWindow(Form1 parent) : base("Desktop Defense Portal", new Rectangle(0, 0, 150, 150))
            {
                Parent = parent;
                DrawEvent += Draw;
            }
            public override void Draw(PaintEventArgs e)
            {
                if (Parent != null)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    e.Graphics.DrawImage(Parent.Portal, new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness, Bounds.Width, Bounds.Height), 0, Parent.PortalSize * frame - 0, Parent.PortalSize, Parent.PortalSize, GraphicsUnit.Pixel);
                    if (Parent.FrameNum % (Parent.FPS / Parent.PortalFPS) == 0)
                        frame++;
                    frame = frame % Parent.PortalFrames;
                }
                base.Draw(e);
            }
        }
        public class GroundWindow : FalseWindow
        {
            public GroundWindow(int i, Form1 parent) : base("Desktop Defense Ground", new Rectangle(100 + (i * 20), 100 + (i * 20), 200, 200), true, true)
            {
                Parent = parent;
                DrawEvent += Draw;
                AddElement(new Anchor(this));
                Elements[0].Draggable = true;
            }
            public override void Draw(PaintEventArgs e)
            {
                if (Parent != null)
                {
                    int spriteSize = Parent.Grass.sprites[0][0].Width;
                    int size = spriteSize * Grass.ScaleFactor;
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                    // Calculate available space
                    int availableWidth = Bounds.Width - size * 2;
                    int availableHeight = Bounds.Height - size * 2;

                    // Calculate how many complete sprites fit in each dimension
                    int completeSpritesX = availableWidth / size;
                    int completeSpritesY = availableHeight / size;

                    // Calculate remaining space for the last partial sprite
                    int remainingX = availableWidth - (completeSpritesX * size);
                    int remainingY = availableHeight - (completeSpritesY * size);

                    // Draw the middle of the ground
                    e.Graphics.DrawImage(Parent.Grass.sprites[1][1], new Rectangle(
                        Bounds.X + size / 2,
                        Bounds.Y + TopFrameThiccness + size / 2,
                        Bounds.Width - size,
                        Bounds.Height - size));

                    // Draw the sides of the ground
                    // Top side (excluding corners)
                    for (int x = 0; x < completeSpritesX; x++)
                    {
                        bool isLastSprite = (x == completeSpritesX - 1) && (remainingX > 0) && (remainingX <= size / 4);
                        int drawWidth = isLastSprite ? size + remainingX : size;

                        e.Graphics.DrawImage(Parent.Grass.sprites[0][1],
                            new Rectangle(Bounds.X + size + (x * size), Bounds.Y + TopFrameThiccness, drawWidth, size),
                            new Rectangle(0, 0, spriteSize, spriteSize),
                            GraphicsUnit.Pixel);
                    }

                    // Handle remaining gap if it's more than 1/4 of a sprite (but not a full sprite)
                    if (remainingX > size / 4 && remainingX < size)
                    {
                        e.Graphics.DrawImage(Parent.Grass.sprites[0][1],
                            new Rectangle(Bounds.X + size + (completeSpritesX * size), Bounds.Y + TopFrameThiccness, remainingX, size),
                            new Rectangle(0, 0, (remainingX * spriteSize) / size, spriteSize),
                            GraphicsUnit.Pixel);
                    }

                    // Bottom side (excluding corners)
                    for (int x = 0; x < completeSpritesX; x++)
                    {
                        bool isLastSprite = (x == completeSpritesX - 1) && (remainingX > 0) && (remainingX <= size / 4);
                        int drawWidth = isLastSprite ? size + remainingX : size;

                        e.Graphics.DrawImage(Parent.Grass.sprites[2][1],
                            new Rectangle(Bounds.X + size + (x * size), Bounds.Y + TopFrameThiccness + Bounds.Height - size, drawWidth, size),
                            new Rectangle(0, 0, spriteSize, spriteSize),
                            GraphicsUnit.Pixel);
                    }

                    // Handle remaining gap if it's more than 1/4 of a sprite
                    if (remainingX > size / 4 && remainingX < size)
                    {
                        e.Graphics.DrawImage(Parent.Grass.sprites[2][1],
                            new Rectangle(Bounds.X + size + (completeSpritesX * size), Bounds.Y + TopFrameThiccness + Bounds.Height - size, remainingX, size),
                            new Rectangle(0, 0, (remainingX * spriteSize) / size, spriteSize),
                            GraphicsUnit.Pixel);
                    }

                    // Left side (excluding corners)
                    for (int y = 0; y < completeSpritesY; y++)
                    {
                        bool isLastSprite = (y == completeSpritesY - 1) && (remainingY > 0) && (remainingY <= size / 4);
                        int drawHeight = isLastSprite ? size + remainingY : size;

                        e.Graphics.DrawImage(Parent.Grass.sprites[1][0],
                            new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness + size + (y * size), size, drawHeight),
                            new Rectangle(0, 0, spriteSize, spriteSize),
                            GraphicsUnit.Pixel);
                    }

                    // Handle remaining gap if it's more than 1/4 of a sprite
                    if (remainingY > size / 4 && remainingY < size)
                    {
                        e.Graphics.DrawImage(Parent.Grass.sprites[1][0],
                            new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness + size + (completeSpritesY * size), size, remainingY),
                            new Rectangle(0, 0, spriteSize, (remainingY * spriteSize) / size),
                            GraphicsUnit.Pixel);
                    }

                    // Right side (excluding corners)
                    for (int y = 0; y < completeSpritesY; y++)
                    {
                        bool isLastSprite = (y == completeSpritesY - 1) && (remainingY > 0) && (remainingY <= size / 4);
                        int drawHeight = isLastSprite ? size + remainingY : size;

                        e.Graphics.DrawImage(Parent.Grass.sprites[1][2],
                            new Rectangle(Bounds.X + Bounds.Width - size, Bounds.Y + TopFrameThiccness + size + (y * size), size, drawHeight),
                            new Rectangle(0, 0, spriteSize, spriteSize),
                            GraphicsUnit.Pixel);
                    }

                    // Handle remaining gap if it's more than 1/4 of a sprite
                    if (remainingY > size / 4 && remainingY < size)
                    {
                        e.Graphics.DrawImage(Parent.Grass.sprites[1][2],
                            new Rectangle(Bounds.X + Bounds.Width - size, Bounds.Y + TopFrameThiccness + size + (completeSpritesY * size), size, remainingY),
                            new Rectangle(0, 0, spriteSize, (remainingY * spriteSize) / size),
                            GraphicsUnit.Pixel);
                    }

                    // Draw the four corners of the ground
                    e.Graphics.DrawImage(Parent.Grass.sprites[0][0], new Rectangle(Bounds.X, Bounds.Y + TopFrameThiccness, size, size));
                    e.Graphics.DrawImage(Parent.Grass.sprites[0][2], new Rectangle(Bounds.X + Bounds.Width - size, Bounds.Y + TopFrameThiccness, size, size));
                    e.Graphics.DrawImage(Parent.Grass.sprites[2][0], new Rectangle(Bounds.X, Bounds.Y + Bounds.Height + TopFrameThiccness - size, size, size));
                    e.Graphics.DrawImage(Parent.Grass.sprites[2][2], new Rectangle(Bounds.X + Bounds.Width - size, Bounds.Y + Bounds.Height + TopFrameThiccness - size, size, size));

                }
                base.Draw(e);
            }
        }
        public class HotbarWindow : FalseWindow
        {
            public HotbarWindow(Form1 parent) : base("Desktop Defense Hotbar", new Rectangle(0, 0, parent.Width / 2, 150))
            {
                Parent = parent;
                DrawEvent += Draw;
            }
            public override void Draw(PaintEventArgs e)
            {
                if (Parent != null)
                {

                }
                base.Draw(e);
            }
        }
        public class CastleWindow : FalseWindow
        {
            public CastleWindow(Form1 parent) : base("Desktop Defense Castle", new Rectangle(0, 0, 150, 150))
            {
                Parent = parent;
                DrawEvent += Draw;
            }
        }
        public class FWndElement
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

            public FWndElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent)
            {
                Bounds = bounds;
                Sprite = sprite;
                Callback = callback;
                State = 0;
                Visible = true;
                Parent = parent;
                Draggable = false;
            }
            public FWndElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent, bool draggable)
            {
                Bounds = bounds;
                Sprite = sprite;
                Callback = callback;
                State = 0;
                Visible = true;
                Parent = parent;
                Draggable = draggable;
            }

            public FWndElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent, int frameCount)
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
            public FWndElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent, int frameCount, bool draggable)
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
        public class Anchor : FWndElement
        {
            public Anchor(FalseWindow parent) : base(new Rectangle(0,0,64,64),Resources.ManaBall,DoNothing,parent,Resources.ManaBall.Height / Resources.ManaBall.Width, true)
            {
                FPS = 16;
            }
            public static void DoNothing()
            {
            }
            public override void Draw(PaintEventArgs e)
            {
                base.Draw(e);
                if (OnFrame)
                {
                    Frame++;
                    State = Frame;
                }
            }
        }
    }
}
