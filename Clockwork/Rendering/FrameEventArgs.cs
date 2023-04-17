using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Rendering
{
    public struct FrameEventArgs
    {
        public readonly Renderer Renderer;
        public readonly GameTime Time;

        public readonly KeyboardState Keyboard;
        public readonly MouseState Mouse;

        public FrameEventArgs(Renderer renderer, GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            Renderer = renderer;
            Time = time; 

            Keyboard = keyboard;
            Mouse = mouse;
        }
    }
}
 