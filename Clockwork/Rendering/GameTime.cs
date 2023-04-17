using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Rendering
{
    public struct GameTime
    {
        // Properties
        public long EllapsedFrames => _ellapsedFrames;

        public long EllapsedMillis => _currentTime;
        public float Ellapsed => EllapsedMillis / 1000f;
        public int EllapsedSeconds => (int)Ellapsed;

        public long DeltaMillis => _currentTime - _oldTime;
        public float Delta => DeltaMillis / 1000f;


        // Values
        private long _currentTime;
        private long _oldTime;
        private long _ellapsedFrames;


        // Constructor
        internal GameTime(long currentTime, long oldTime, long ellapsedFrames)
        {
            _currentTime = currentTime;
            _oldTime = oldTime;
            _ellapsedFrames = ellapsedFrames;
        }
    }
}
