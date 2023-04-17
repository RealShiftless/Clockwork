using Clockwork;
using Clockwork.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingUnit
{
    public class PerformanceMonitor
    {
        private long _lastFrame = 0;
        private float _lastEllapsed = 0;

        private float _t;

        public PerformanceMonitor()
        {
            Program.Game.Renderer.Update += Update;
        }

        private void Update(object sender, FrameEventArgs e)
        {
            _t += e.Time.Delta;

            if (_t > 5)
            {
                Debug.WriteLine("Current Frame: " + e.Time.EllapsedFrames + " Current Time: " + e.Time.Ellapsed + " Avg FPS: " + (int)Math.Round((e.Time.EllapsedFrames - _lastFrame) / (e.Time.Ellapsed - _lastEllapsed)));

                _lastFrame = e.Time.EllapsedFrames;
                _lastEllapsed = e.Time.Ellapsed;

                _t = 0;
            }
        }
    }
}
