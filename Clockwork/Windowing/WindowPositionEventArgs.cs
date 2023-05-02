using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Windowing
{
    public struct WindowPositionEventArgs
    {
        public readonly int PreviousX;
        public readonly int PreviousY;

        public readonly int X;
        public readonly int Y;

        public int DeltaX => X - PreviousX;
        public int DeltaY => Y - PreviousY;

        public WindowPositionEventArgs(int prevX, int prevY, int x, int y)
        {
            PreviousX = prevX;
            PreviousY = prevY;

            X = x;
            Y = y;
        }
    }
}
