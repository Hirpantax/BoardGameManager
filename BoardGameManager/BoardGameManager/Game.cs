using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameManager
{
    public abstract class Game
    {
        protected string gameName;
        protected int playerCount;

        public abstract void PlayGame();
    }
}
