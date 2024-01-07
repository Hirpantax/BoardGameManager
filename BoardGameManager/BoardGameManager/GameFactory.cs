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

                    return new UnoFlip();
                    
            }
        }
        
    }
    
}
