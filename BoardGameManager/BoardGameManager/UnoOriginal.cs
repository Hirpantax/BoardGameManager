using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameManager
{
    public class UnoOriginal : Game
    {
        private enum GameMode
        {
            highestIsWinner, //The WINNER is the first player to reach 500 points
            lastOneStanding //When a player reaches 500 points, he/she is eliminated. Game goes on untill one player remains under the 500 points (2+ players)
        }
        private Stack<ICommand> commandHistory = new Stack<ICommand>();
        private const int pointCap = 500;
        private GameMode gameMode;
        private List<string> playerNameList = new List<string>();
        private List<int> playerPointList = new List<int>();
        private int playerInitialPoint = 0;
        private int roundCount = 0;

        public UnoOriginal() 
        {
            gameName = "Uno Original";
        }

        public override void UpdatePlayerPoints(int playerID, int points)
        {
            playerPointList[playerID] = points;
        }

        public override void ExecuteCommand(ICommand command)
        {
            command.Execute();
            commandHistory.Push(command);
        }
        public override void UndoLastCommand()
        {
            if (commandHistory.Any())
            {
                var lastCommand = commandHistory.Pop();
                lastCommand.Undo();
            }
        }

        public override void PlayGame()
        {
            throw new NotImplementedException();
        }




    }
}
