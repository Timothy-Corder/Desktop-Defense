using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TimUtils;

namespace Desktop_Defense
{
    namespace Utils
    {
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
                    }
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
        public class TowerWindow : FalseWindow
        {
            public TowerWindow(Form1 parent) : base("Desktop Defense Tower", new Rectangle(0, 0, 150, 150))
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
                    FalseWindow fWnd;
                    _parentRef.TryGetTarget(out fWnd);
                    return fWnd;
                }
                set
                {
                    _parentRef = new WeakReference<FalseWindow>(value);
                }
            }
            public bool StaticSprite = true;
            public int FrameCount = 1;
            public int Frame = 0;
            public int FPS;
            public Size spriteSize;

            public FWndElement(Rectangle bounds, Image sprite, Action callback, FalseWindow parent)
            {
                Bounds = bounds;
                Sprite = sprite;
                Callback = callback;
                State = 0;
                Visible = true;
                Parent = parent;
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
            }
            public void Draw(PaintEventArgs e)
            {
                if (Parent != null)
                
                    if (Visible)
                    {
                        e.Graphics.DrawImage(Sprite, Bounds, 0, (Sprite.Height / 3) * State, Sprite.Width, Sprite.Height / 3, GraphicsUnit.Pixel);
                    }
                }
            }
            public void Trigger()
            {
                Callback.Invoke();
            }
        }
    }
}
