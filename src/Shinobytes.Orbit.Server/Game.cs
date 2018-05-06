using System;
using System.Collections.Generic;
using System.Text;
using Shinobytes.Core;

namespace Shinobytes.Orbit.Server
{
    public class Game : IGame
    {
        private readonly ILogger logger;

        public Game(ILogger logger)
        {
            this.logger = logger;
        }

        public void Begin()
        {
        }
    }
}
