using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameManager
{
    public class UnoFlip : Game
    {
        private enum GameMode
        {
            highestIsWinner, //The WINNER is the first player to reach 500 points
            lowestIsWinner //When one player reaches 500 points, the player with the lowest points is the winner.
        }
        private Stack<ICommand> commandHistory = new Stack<ICommand>();
        private const int pointCap = 500;
        private GameMode gameMode;
        private List<string> playerNameList = new List<string>();
        private List<int> playerPointList = new List<int>();
        private int playerInitialPoint = 0;
        private int roundCount = 1;
        private const string FILE_PATH = "UnoFlip.txt";
        private const string RULES_HEADER = "Rules:";
        private const string ACTION_CARDS_HEADER = "Action Cards:";
        private const string SCORING_HEADER = "Scoring:";
        private const string HIGHEST_IS_WINNER_HEADER = "Highest is the Winner:";
        private const string LOWEST_IS_WINNER_HEADER = "Lowest is the Winner:";

        public UnoFlip()
        {
            gameName = "Uno Flip";
            minPlayerCount = 2;
            maxPlayerCount = 10;
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
           InitializeGame();

           
           int pointCapPlayer = IsGameOver();
            bool isRoundOver = false;

            while (pointCapPlayer == -1)
            {
                while (!isRoundOver)
                {
                    isRoundOver = PrintEndOfRoundMenu();
                }
                ProgressOneRound();
                pointCapPlayer = IsGameOver();
                isRoundOver = false;
            }

            Console.WriteLine();
            switch (gameMode)
            {
                case GameMode.highestIsWinner:
                    
                    Console.WriteLine("{0} IS THE WINNER!!", playerNameList[pointCapPlayer]);

                    break;

                case GameMode.lowestIsWinner:
                    List<int> winnerIDs = FindMinWinners();

                    if (winnerIDs.Count > 1)
                    {
                        Console.Write("THE WINNERS ARE: ");

                        for (int i = 0; i < winnerIDs.Count; i++)
                        {
                            Console.Write(playerNameList[winnerIDs[i]] + ", ");
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} IS THE WINNER!!", playerNameList[winnerIDs[0]]);
                    }
                    break;
            }
            Console.WriteLine();
            Console.WriteLine("Play again? (Y/n) ");
            string input = Console.ReadLine();

            while (!(!string.IsNullOrEmpty(input) && 
                input.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("n", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Invalid input. Please enter again: ");
                input = Console.ReadLine();
            }

            if (input.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                ResetGame();
            }
            
        }

        private void ProgressOneRound()
        {
            //roundCount++;
            //Console.WriteLine("ROUND: {0}", roundCount);
            PrintPlayerList(false);
            Console.WriteLine();
            Console.Write("Enter the ID of the player who won the round {0}: ", roundCount);
            string roundWinnerID = Console.ReadLine();
            int winnerID;

            while (!(int.TryParse(roundWinnerID, out winnerID)) || !IsIDInRange(winnerID))
            {
                
                Console.Write("Invalid input. Please enter again: ");
                roundWinnerID = Console.ReadLine();
            }
            winnerID--; //index

            int pointSum = 0; //Sum of the losers' points
            int point;
            int previousPoints;
            int newPoints;
            switch (gameMode)
            {
                case GameMode.highestIsWinner:

                    for (int i = 0; i < playerCount; i++)
                    {
                        if (i == winnerID) continue;
                        Console.Write("Enter the amount of points {0} is left with: ", playerNameList[i]);
                        
                        while (!int.TryParse(Console.ReadLine(), out point) || point < 0)
                        {
                            Console.Write("Invalid input. Please enter again: ");
                        }
                        pointSum += point;
                    }
                    
                    previousPoints = playerPointList[winnerID];
                    newPoints = previousPoints + pointSum;
                    UpdatePointsCommand updateCommandHighest = new UpdatePointsCommand(this, winnerID, previousPoints, newPoints);
                    ExecuteCommand(updateCommandHighest);
                    
                    //playerPointList[winnerID] += pointSum;

                    break;

                case GameMode.lowestIsWinner:
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (i == winnerID) continue;
                        Console.Write("Enter the points {0} is left with: ", playerNameList[i]);

                        while (!int.TryParse(Console.ReadLine(), out point) || point < 0)
                        {
                            Console.Write("Invalid input. Please enter again: ");
                        }
                        
                        previousPoints = playerPointList[i];
                        newPoints = previousPoints + point;
                        UpdatePointsCommand updateCommandLowest = new UpdatePointsCommand(this, i, previousPoints, newPoints);
                        ExecuteCommand(updateCommandLowest);
                        
                        //playerPointList[i] += point;
                    }

                    break;
            }

            PrintPlayerList(true);
            Console.Write("Press U to undo the last round, or any other key to continue.");
            if (Console.ReadKey().Key == ConsoleKey.U)
            {
                switch (gameMode)
                {
                    case GameMode.highestIsWinner:
                        UndoLastCommand();
                        break;

                    case GameMode.lowestIsWinner:
                        for (int i = 0; i < playerCount - 1; i++)
                        {
                            UndoLastCommand();
                        }
                        break;
                }
                
                Console.WriteLine();
                Console.WriteLine("Last round undone.");
                //roundCount--;
            }
            else
            {
                roundCount++;
            }

            Console.WriteLine();
        }

        private void PrintFileSection(string filePath, string sectionHeader)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    bool printLines = false;
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        // Check if the current line is a section header
                        if (line.Trim().Equals(sectionHeader))
                        {
                            printLines = true; // Start printing lines
                            //continue; // Skip the header line
                        }

                        // Check for another section header to stop printing
                        if (printLines && line.EndsWith(":") && !line.Trim().Equals(sectionHeader))
                        {
                            break;
                        }

                        if (printLines)
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        private void PrintPlayerList(bool includePoints)
        {
            switch (includePoints)
            {
                case true:
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Player ID\tPlayer Name\tPoints");


                    for (int i = 0; i < playerCount; i++)
                    {
                        Console.Write(i + 1 + "\t\t");
                        Console.Write(playerNameList[i] + "\t\t");
                        Console.WriteLine(playerPointList[i]);
                    }
                    break;

                case false:
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Player ID\tPlayer Name");


                    for (int i = 0; i < playerCount; i++)
                    {
                        Console.Write(i + 1 + "\t\t");
                        Console.WriteLine(playerNameList[i] + "\t\t");
                    }
                    break;
            }

        }

        private void InitializeGame()
        {
            Console.WriteLine();
            Console.WriteLine("Game Modes:");

            PrintFileSection(FILE_PATH, HIGHEST_IS_WINNER_HEADER);
            PrintFileSection(FILE_PATH, LOWEST_IS_WINNER_HEADER);

            string[] gameModeNames = Enum.GetNames(typeof(GameMode));
            GameMode[] gameModes = (GameMode[])Enum.GetValues(typeof(GameMode));
            int gameMode;

            for (int i = 0; i < gameModeNames.Length; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, gameModeNames[i]);
            }

            Console.Write("Enter the ID of the game mode you want to play: ");

            while (!int.TryParse(Console.ReadLine(), out gameMode) || !(gameMode >= 1 && gameMode <= gameModeNames.Length))
            {
                Console.WriteLine("Invalid input. Please enter again: ");
            }

            gameMode--; //Index

            this.gameMode = gameModes[gameMode];

            PrintFileSection(FILE_PATH, RULES_HEADER);

            string playerNameTemp;
            bool playerCountConfirmation = false;

            while (!playerCountConfirmation)
            {
                Console.Write("Enter the amount of players: ");

                while (!int.TryParse(Console.ReadLine(), out playerCount) || !(playerCount >= minPlayerCount && playerCount <= maxPlayerCount))
                {
                    Console.Write("Invalid input. Please enter again: ");
                }
                Console.Write("{0} player profiles will be created. Press Y if you confirm, or any other key if you don't: ", playerCount);

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    playerCountConfirmation = true;
                }
                Console.WriteLine();    
            }
        

           
            for (int i = 0; i < playerCount; i++)
            {
                Console.Write("Enter the name of the player number {0}: ", i + 1);
                playerNameTemp = Console.ReadLine();

                while (string.IsNullOrEmpty(playerNameTemp))
                {
                    Console.WriteLine("Invalid input. Please enter again: ");
                    playerNameTemp = Console.ReadLine();
                }
                playerNameList.Add(playerNameTemp);
                playerPointList.Add(playerInitialPoint);
            }

        }

        private bool PrintEndOfRoundMenu()
        {
            Console.WriteLine();
            Console.WriteLine("ROUND: {0}", roundCount);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("1. Display Action Card Info");
            Console.WriteLine("2. Display Card Values");
            Console.WriteLine("3. End Round");
            Console.Write("Enter the ID of your choice: ");
            int menuOption;

            while (!int.TryParse(Console.ReadLine(), out menuOption) || !(menuOption >=1 && menuOption <=3))
            {
                Console.Write("Invalid input. Please enter again: ");
            }

            switch (menuOption)
            {
                default:
                case 1:
                    PrintFileSection(FILE_PATH, ACTION_CARDS_HEADER);
                    return false;

                case 2:
                    PrintFileSection(FILE_PATH, SCORING_HEADER);
                    return false;

                case 3:
                    return true;

            }

        }

        private int IsGameOver()
        {
            // Has any of the players reached the point cap
            for (int i = 0; i < playerCount; i++)
            {
                if (playerPointList[i] >= pointCap)
                {
                    return i;
                }

            }
            return -1;
        }

        private bool IsIDInRange (int playerInputID)
        {
            if (playerInputID >= 1 && playerInputID <= playerCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<int> FindMinWinners()
        {
            List<int> winners = new List<int>();
            int minPoint = playerPointList.Min();

            for (int i = 0; i< playerCount; i++)
            {
                if (playerPointList[i] == minPoint)
                {
                    winners.Add(i);
                }
            }
            return winners;
        }

        private void ResetGame()
        {
            for (int i = 0;i < playerCount;i++)
            {
                playerPointList[i] = 0;
            }
            roundCount = 0;
            Console.Clear();
            PlayGame();
        }


    }
}
