using AwFuL.PlayingCard;

Console.WriteLine(Environment.NewLine + "*******************************" + Environment.NewLine + "Ninety Nine - A Card Game" + Environment.NewLine);

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
    int activePlayerIndex = 0;
    int discardTotal = 0;

    // round loop
    do
    {
        if (playerTokens[activePlayerIndex] == 0)
        {
            continue;
        }

        Console.WriteLine(Environment.NewLine + $"Current discard total is {discardTotal}");

        if (activePlayerIndex == 0)
        {
            for (int cc = 0; cc < 3; cc++)
            {
                Console.WriteLine($"\t {cc + 1} {playerHands[0][cc]}");
            }
            int selectedMenuCard = 0;
            do
            {
                Console.Write($"Which card would you like to play, {playerName}? ");
            } while (!int.TryParse(Console.ReadLine(), out selectedMenuCard) || selectedMenuCard < 1 || selectedMenuCard > 3);
            StandardRank selectedCardRank = playerHands[activePlayerIndex][selectedMenuCard - 1].Rank;
            int cardValue = selectedCardRank switch
            {
                StandardRank.Ace => SelectCardValue([11, 1]),
                StandardRank.Ten => SelectCardValue([-10, 10]),
                StandardRank.Jack or StandardRank.Queen => 10,
                StandardRank.Four or StandardRank.King => 0,
                StandardRank.Nine => 99,
                _ => (int)selectedCardRank,
            };
            discardTotal = cardValue == 99 ? 99 : discardTotal + cardValue;

            playerHands[activePlayerIndex].RemoveAt(selectedMenuCard - 1);
            playerHands[activePlayerIndex].Add(drawPile.DrawCard());

            if (selectedCardRank == StandardRank.Three)
            {
                Console.WriteLine($"{playerName} skipped {players[activePlayerIndex + 1]}!");
                activePlayerIndex++;
            }
        }
        else
        {
            Console.WriteLine($"{players[activePlayerIndex]}'s turn.");
        }

        // TODO: handle reverse (4)
        activePlayerIndex++;
        if (activePlayerIndex == players.Count)
        {
            activePlayerIndex = 0;
        }

        // TODO: proper endRound due to no valid cards to play
        endRund = discardTotal >= 99;
    } while (!endRund);
    Console.WriteLine(Environment.NewLine + $"Current discard total is {discardTotal}");

    /****************************************
    **                                     **
    **  HANDLE NOT IMPLEMENTED CODE STATE  **
    **                                     **
    ****************************************/
    break;
    /****************************************
    **                                     **
    **  HANDLE NOT IMPLEMENTED CODE STATE  **
    **                                     **
    ****************************************/
} while (playerTokens.Count(t => t > 0) > 1);

Console.WriteLine(Environment.NewLine + $"Press any key to exit, {playerName}.");
Console.ReadKey();

static int SelectCardValue(int[] options)
{
    int selectedValue = -99;
    do
    {
        Console.Write($"{options[0]} or {options[1]}? ");
    } while (!int.TryParse(Console.ReadLine(), out selectedValue) || !options.Contains(selectedValue));
    return selectedValue;
}