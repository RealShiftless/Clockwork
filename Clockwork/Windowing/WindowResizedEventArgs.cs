using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Windowing
{
    public struct WindowResizedEventArgs
    {
        // Values
        public readonly int PreviousWidth;
        public readonly int PreviousHeight;

        public readonly int Width;
        public readonly int Height;


        // Properties
        public int DeltaWidth => Width - PreviousWidth;
        public int DeltaHeight => Height - PreviousHeight;


        // Constructor
        public WindowResizedEventArgs(int prevWidth, int prevHeight, int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
