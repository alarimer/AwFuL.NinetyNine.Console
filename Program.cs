// See https://aka.ms/new-console-template for more information
using AwFuL.PlayingCard;

Console.WriteLine("Ninety Nine - A Card Game");

string playerName;
Dictionary<int, string> players = [];
do
{
    Console.Write("What is your name? ");
    playerName = Console.ReadLine()?.Trim() ?? String.Empty;
} while (String.IsNullOrEmpty(playerName));
players.Add(0, playerName);
Console.WriteLine(Environment.NewLine + $"Hello, {playerName}.");

int computerOpponentCount;
do
{
    Console.Write("How many computer opponents would you like to have: 1 to 4? ");
} while (!int.TryParse(Console.ReadLine(), out computerOpponentCount) || computerOpponentCount < 1 || computerOpponentCount > 4);
for (int coc = 1; coc <= computerOpponentCount; coc++)
{
    players.Add(coc, $"computer{coc}");
}
Console.WriteLine($"Okay, {playerName}, {computerOpponentCount} computer opponents. Good luck!");

int[] playerTokens = new int[players.Count];
Array.Fill(playerTokens, 3);

// game loop
do
{
    CardDeck drawPile = CardDeck.CreateStandard52CardDeck();
    drawPile.Shuffle();
    Dictionary<int, List<StandardCard>> playerHands = [];
    foreach (KeyValuePair<int, string> p in players)
    {
        if (playerTokens[p.Key] == 0)
        {
            continue;
        }

        List<StandardCard> hand = [];
        for (int lc = 0; lc < players.Count; lc++)
        {
            hand.Add(drawPile.DrawCard());
        }
        playerHands.Add(p.Key, hand);
    }

    Console.WriteLine(Environment.NewLine + "*******************************" + Environment.NewLine + $"Begin round!");
    bool endRund = false;
    // round loop
    do
    {

        // HANDLE UNSTABLE CODE STATE
        endRund = true;
    } while (!endRund);

    // HANDLE UNSTABLE CODE STATE
    break;
} while (playerTokens.Count(t => t > 0) > 1);

Console.WriteLine($"Press any key to exit, {playerName}.");
Console.ReadKey();