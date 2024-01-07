using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameManager
{
    public class UpdatePointsCommand : ICommand
    {
        private Game game;
        private int playerID;
        private int previousPoints;
        private int newPoints;

        public UpdatePointsCommand(Game game, int playerID, int previousPoints, int newPoints)
        {
            this.game = game;
            this.playerID = playerID;
            this.previousPoints = previousPoints;
            this.newPoints = newPoints;
        }

        public void Execute()
        {
            game.UpdatePlayerPoints(playerID, newPoints);
        }

        public void Undo()
        {
            game.UpdatePlayerPoints(playerID, previousPoints);
        }
    }
}
