using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using TimUtils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Desktop_Defense
{
    namespace Utils
    {

        public class Screenshotter
        {
            public WeakReference<GameForm> Parent;
            public Screenshotter(GameForm p)
            {
                Parent = new WeakReference<GameForm>(p);
            }
            public Bitmap? TakeScreenshot(Screen screen)
            {
                if (Parent != null)
                {
                    GameForm form;
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
                get => Intify(_value);
                set => _value = Intify(value);
            }
            public IntVector(Vector2 v)
            {
                _value = Intify(v);
            }
            public IntVector(int x, int y)
            {
                _value.X = x;
                _value.Y = y;
            }
            public static implicit operator Vector2(IntVector v)
            {
                return v.Value;
            }
            public static implicit operator IntVector(Vector2 v)
            {
                return new IntVector(v);
            }
            private Vector2 Intify(Vector2 v)
            {
                return new Vector2((int)_value.X, (int)_value.Y);
            }
        }
        public class ImgButton
        {
            public Rectangle Bounds;
            public bool Visible;
            public int State;
            public Bitmap Sprite;
            public Action Callback;
            public WeakReference<GameForm> Parent;
            public ImgButton(Rectangle bounds, Bitmap sprite, Action callback, WeakReference<GameForm> parent)
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
                GameForm form;
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
    }
}
