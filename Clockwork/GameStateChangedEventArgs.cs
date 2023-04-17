﻿using Clockwork.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork
{
    public struct GameStateChangedEventArgs
    {
        public readonly GameState OldState;
        public readonly GameState NewState;

        public readonly FrameEventArgs FrameEvent;

        public GameStateChangedEventArgs(GameState oldState, GameState newState, FrameEventArgs e)
        {
            OldState = oldState;
            NewState = newState;
            FrameEvent = e;
        }
    }
}
