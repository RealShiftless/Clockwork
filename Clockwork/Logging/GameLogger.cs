using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Logging
{
    public class GameLogger : ILoggerType
    {
        public void Initialize(Logger logger)
        {
            logger.Game.Starting += () => logger.Log(logger.Game, "Starting...");
            logger.Game.Stopped += () => logger.Log(logger.Game, "Stopped");

            logger.Game.Initializing += () => logger.Log(logger.Game, "Initializing...");
            logger.Game.Initialized += () => logger.Log(logger.Game, "Initialized");

            logger.Game.StateChanged += (s, e) => logger.Log(logger.Game, "State changed...");
        }

        private void Game_StateChanged(Game sender, GameStateChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
