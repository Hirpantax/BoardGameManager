using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameManager
{
    public static class GameFactory
    {
        private static readonly List<string> unoFlipGameModeNames = new List<string> { "Highest is Winner", "Lowest is Winner" };
        public static Game CreateGame(int gameID)
        {
            switch (gameID)
            {
                default:
                case 1: //Uno Flip
                    int gameMode;
                    Console.WriteLine("Game Modes:");

                    for (int i = 0; i <unoFlipGameModeNames.Count; i++)
                    {
                        Console.WriteLine("{0}. {1}", i + 1, unoFlipGameModeNames[i]);
                    }
                    Console.Write("Enter the number of the game mode you want to play: ");

                    while (!int.TryParse(Console.ReadLine(), out gameMode) || !(gameMode >= 1 && gameMode <= unoFlipGameModeNames.Count))
                    {
                        Console.WriteLine("Invalid input. Please enter again: ");
                    }

                    return new UnoFlip(gameMode);
                    
            }
        }
        
    }
    
}
