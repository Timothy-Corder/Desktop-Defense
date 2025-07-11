using Desktop_Defense.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Defense
{
    public class PortalWindow : FalseWindow
    {
        public PortalWindow(GameForm parent) : base("Desktop Defense Portal", new Rectangle(0, 0, 150, 150))
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
        public int AnchorCount;
        public GroundWindow(int i, GameForm parent, int anchorCount = 3) : base("Desktop Defense Ground", new Rectangle(100 + (i * 20), 100 + (i * 20), 400, 400), true, true)
        {
            Parent = parent;
            DrawEvent += Draw;
            AnchorCount = anchorCount;
            for (int j = 0; j < AnchorCount; j++)
            {
                AddElement(new Anchor(this, (j * 64) % this.Bounds.Width, (j * 64) / this.Bounds.Height));
                Elements[j].Draggable = true;
            }
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
        public HotbarWindow(GameForm parent) : base("Desktop Defense Hotbar", new Rectangle(0, 0, parent.Width / 2, 150))
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
        public CastleWindow(GameForm parent) : base("Desktop Defense Castle", new Rectangle(0, 0, 150, 150))
        {
            Parent = parent;
            DrawEvent += Draw;
        }
    }
}
