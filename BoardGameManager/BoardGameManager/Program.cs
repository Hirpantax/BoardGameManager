

using BoardGameManager;

public class Program
{
    static void Main(string[] args)
    {
        List<string> testList = new List<string> { "arda", "ayca", "meltem" };
        UnoFlip unoFlip = new UnoFlip(1, 3, testList);

        unoFlip.PlayGame();
    }
}