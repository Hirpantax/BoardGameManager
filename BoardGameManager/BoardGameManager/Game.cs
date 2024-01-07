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

        public abstract void UpdatePlayerPoints(int playerID, int points);

        public abstract void ExecuteCommand(ICommand command);

        public abstract void UndoLastCommand();
    }
}
