using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Defense
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
}
