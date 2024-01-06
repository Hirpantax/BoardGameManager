

using BoardGameManager;

public class Program
{
    private static List<string> gameList = new List<string> { "Uno Flip", "Classic Uno" };
    static void Main(string[] args)
    {
        Game game;
        Console.WriteLine("Welcome!");
        Console.WriteLine();
        PrintGames();

        Console.Write("Please enter the ID of the game you want to play: ");

        int gameID;
        while (!int.TryParse(Console.ReadLine(), out gameID) || !(gameID >= 1 && gameID <= gameList.Count))
        {
            Console.WriteLine("Invalid input. Please enter again: ");
        }

        game = GameFactory.CreateGame(gameID);

        game.PlayGame();


    }

    private static void PrintGames()
    {
        for (int i = 0; i < gameList.Count; i++)
        {
            Console.WriteLine("{0}. {1}", i + 1, gameList[i]);
        }
    }
}