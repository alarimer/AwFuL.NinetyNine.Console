using System.Linq.Expressions;
using System.Runtime.InteropServices;
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
Thread.Sleep(2000);

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
        for (int lc = 3; lc > 0; lc--)
        {
            hand.Add(drawPile.DrawCard());
        }
        playerHands.Add(p.Key, hand);
    }

    Console.WriteLine(Environment.NewLine + "*******************************" + Environment.NewLine + $"Begin round!");
    Thread.Sleep(2000);
    bool endRound = false;
    int activePlayerIndex = 0;
    int discardTotal = 0;
    bool isForwardPlay = true;

    // round loop
    do
    {
        if (playerTokens[activePlayerIndex] == 0)
        {
            activePlayerIndex = AdvancePlay(players.Count, activePlayerIndex, isForwardPlay);
            continue;
        }

        Console.WriteLine(Environment.NewLine + $"Current discard total is {discardTotal}");

        bool hasValidPlay = false;
        foreach (StandardCard card in playerHands[activePlayerIndex])
        {
            hasValidPlay = card.Rank switch
            {
                StandardRank.Four or StandardRank.King or StandardRank.Nine or StandardRank.Ten => true,
                StandardRank.Ace => discardTotal + 1 < 100,
                StandardRank.Jack or StandardRank.Queen => discardTotal + 10 < 100,
                _ => discardTotal + (int)card.Rank < 100,
            };

            if (hasValidPlay)
            {
                break;
            }
        }
/*******************************************************
****                                                ****
****    HACK: INCOMPLETE IMPLEMENTATION INSURANCE   ****
****                                                ****
*******************************************************/
if (discardTotal > 88 && activePlayerIndex != 0) hasValidPlay = false;
/*******************************************************
****                                                ****
****    HACK: INCOMPLETE IMPLEMENTATION INSURANCE   ****
****                                                ****
*******************************************************/
        if (!hasValidPlay)
        {
            Console.WriteLine($"{players[activePlayerIndex]} cannot play a card and loses a token.");
            Console.WriteLine($"\t{playerHands[activePlayerIndex][0]} - {playerHands[activePlayerIndex][1]} - {playerHands[activePlayerIndex][2]}" + Environment.NewLine);
            playerTokens[activePlayerIndex]--;
            Thread.Sleep(3000);
            for (int pc = 0; pc < players.Count; pc++)
            {
                Console.WriteLine($"{players[pc],-10}: " + (playerTokens[pc] > 0 ? $"{playerTokens[pc]} tokens" : "out"));
            }
            Thread.Sleep(3000);
            endRound = true;
            break;
        }

        if (activePlayerIndex == 0)
        {
            for (int cc = 0; cc < 3; cc++)
            {
                Console.WriteLine($"\t {cc + 1} {playerHands[0][cc]}");
            }
            bool isValidCard = false;
            StandardCard selectedCard;
            StandardRank selectedCardRank;
            int cardValue;
            do
            {
                int selectedMenuCard = 0;
                do
                {
                    Console.Write($"Which card would you like to play, {playerName}? ");
                } while (!int.TryParse(Console.ReadLine(), out selectedMenuCard) || selectedMenuCard < 1 || selectedMenuCard > 3);
                selectedCard = playerHands[activePlayerIndex][selectedMenuCard - 1];
                selectedCardRank = selectedCard.Rank;
                cardValue = selectedCardRank switch
                {
                    StandardRank.Ace => SelectCardValue([11, 1]),
                    StandardRank.Ten => SelectCardValue([-10, 10]),
                    StandardRank.Jack or StandardRank.Queen => 10,
                    StandardRank.Four or StandardRank.King => 0,
                    StandardRank.Nine => 99,
                    _ => (int)selectedCardRank,
                };
                isValidCard = cardValue == 99 || discardTotal + cardValue < 100;
                if (!isValidCard)
                {
                    Console.WriteLine($"Cannot play the {selectedCard} because the discard total would be {discardTotal + cardValue} which exceeds 99.");
                }
            } while (!isValidCard);
            Console.WriteLine(Environment.NewLine + $"{playerName} played {selectedCard} for " + (cardValue == 99 ? $"{cardValue - discardTotal} resulting in a discard total of 99" : $"{cardValue}"));
            discardTotal = cardValue == 99 ? 99 : discardTotal + cardValue;

            playerHands[activePlayerIndex].Remove(selectedCard);
            playerHands[activePlayerIndex].Add(drawPile.DrawCard());

            if (selectedCardRank == StandardRank.Three)
            {
                Console.WriteLine($"{playerName} skipped {(isForwardPlay ? players[activePlayerIndex + 1] : players[players.Count - 1])}!");
                if (isForwardPlay)
                {
                    activePlayerIndex++;
                }
                else
                {
                    activePlayerIndex = players.Count - 1;
                }
            }

            if (selectedCardRank == StandardRank.Four)
            {
                Console.WriteLine($"{playerName} reversed play!");
                isForwardPlay = !isForwardPlay;
            }
        }
        else
        {
            Console.WriteLine($"{players[activePlayerIndex]}'s turn.");
            Thread.Sleep(3000);
        }

        activePlayerIndex = AdvancePlay(players.Count, activePlayerIndex, isForwardPlay);
    } while (!endRound);
} while (playerTokens.Count(t => t > 0) > 1);

Console.WriteLine(Environment.NewLine + $"{players[playerTokens.ToList().FindIndex(t => t > 0)]} is the winner!");
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

static int AdvancePlay(int playerCount, int activePlayerIndex, bool isForwardPlay)
{
    if (isForwardPlay)
    {
        activePlayerIndex++;
        if (activePlayerIndex == playerCount)
        {
            activePlayerIndex = 0;
        }
    }
    else
    {
        activePlayerIndex--;
        if (activePlayerIndex == -1)
        {
            activePlayerIndex = playerCount - 1;
        }
    }

    return activePlayerIndex;
}