using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Defense
{
    public class Anchor : FalseElement
    {
        public Anchor(FalseWindow parent, int x, int y) : base(new Rectangle(x, y, 64, 64), Resources.ManaBall, DoNothing, parent, Resources.ManaBall.Height / Resources.ManaBall.Width, true)
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
