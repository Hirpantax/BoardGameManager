using System;
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

        private const int pointCap = 500;
        private GameMode gameMode;
        private List<string> playerNameList = new List<string>();
        private List<int> playerPointList = new List<int>();
        private int playerInitialPoint = 0;
        private int roundCount = 0;

        public UnoFlip(int gameMode)
        {
            gameName = "Uno Flip";
            
            switch (gameMode)
            {
                default:
                case 1:
                    this.gameMode = GameMode.highestIsWinner;
                    break;

                case 2:
                    this.gameMode= GameMode.lowestIsWinner;
                    break;

            }

        }

        public override void PlayGame()
        {
           InitializeGame();

           int pointCapPlayer = IsGameOver();
           while (pointCapPlayer == -1)
            {
                ProgressOneRound();
                pointCapPlayer = IsGameOver();
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
            roundCount++;
            Console.WriteLine("ROUND: {0}", roundCount);
            PrintPlayerList();
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

                    //Add the pointSum to winner's points
                    playerPointList[winnerID] += pointSum;

                    break;

                case GameMode.lowestIsWinner:
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (i == winnerID) continue;
                        Console.WriteLine("Enter the points for {0}: ", playerNameList[i]);

                        while (!int.TryParse(Console.ReadLine(), out point))
                        {
                            Console.WriteLine("Invalid input. Please enter again: ");
                        }
                        playerPointList[i] += point;
                    }

                    break;
            }

            Console.WriteLine();
        }

        private void PrintPlayerList()
        {
            Console.WriteLine("--------------------------------------");
            //Console.WriteLine("Player List");
            //Console.WriteLine("--------------------------------------");
            Console.WriteLine("Player ID\tPlayer Name\tPoints");


            for (int i = 0; i < playerCount; i++)
            {
                Console.Write(i + 1 + "\t\t");
                Console.Write(playerNameList[i] + "\t\t");
                Console.WriteLine(playerPointList[i]);
            }
        }

        private void InitializeGame()
        {
            string playerNameTemp;
            Console.Write("Enter the amount of players: ");
            
            while (!int.TryParse(Console.ReadLine(), out playerCount))
            {
                Console.WriteLine("Invalid input. Please enter again: ");
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
