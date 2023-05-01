using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Logging
{
    public class RendererLogger : ILoggerType
    {
        public void Initialize(Logger logger)
        {
            logger.Game.Renderer.Load += () => logger.Log(logger.Game.Renderer, "Loading...");
            logger.Game.Renderer.RenderThreadStarted += () => logger.Log(logger.Game.Renderer, "Render Thread Started");

            logger.Game.Renderer.Closing += (e) => logger.Log(logger.Game.Renderer, "Closing...");

            logger.Game.Renderer.Unload += () => logger.Log(logger.Game.Renderer, "Unloading...");

            logger.Game.Renderer.FileDrop += (e) =>
            {
                string files = "{ ";

                for(int i = 0; i < files.Length; i++)
                {
                    if(i != 0)
                        files += ", ";

                    files += e.FileNames[i];
                }

                files += " }";

                logger.Log(logger.Game.Renderer, "Files dropped " + files);
            };
        }

    }
}
