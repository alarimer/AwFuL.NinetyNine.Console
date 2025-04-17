/*
*   Copyright (c) 2025 Alan Larimer
*   MIT License
*   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED ...
*/

using AwFuL.PlayingCard;

Console.WriteLine(Environment.NewLine + "*******************************" + Environment.NewLine + "Ninety Nine - A Card Game" + Environment.NewLine);
Thread.Sleep(3000);

string playerName;
Dictionary<int, string> players = [];
do
{
    Console.Write("What is your name? ");
    playerName = Console.ReadLine()?.Trim() ?? String.Empty;
} while (string.IsNullOrEmpty(playerName));
players.Add(0, playerName);
Console.WriteLine($"Hello, {playerName}.");
Thread.Sleep(2000);

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
while (playerTokens.Count(t => t > 0) > 1)
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

    var activePlayerIndexes = playerTokens.Select((value, index) => new {value, index}).Where(t => t.value > 0).Select(p => p.index).ToArray();
    int activePlayerIndex = activePlayerIndexes[0];
    int discardTotal = 0;
    bool isForwardPlay = true;

    // round loop
    while (true)
    {
        string currentPlayerName = players[activePlayerIndex];

        Console.WriteLine(Environment.NewLine + $"Current discard total is {discardTotal}");

        // check for three of a kind "Michigan variation"
        if (IsMichiganWin(playerHands[activePlayerIndex]))
        {
            Console.WriteLine($"{currentPlayerName} has three of a kind and wins the round!" + Environment.NewLine + "All other players lose a token.");
            PrintPlayerHand(playerHands[activePlayerIndex]);
            for (int pt = playerTokens.Length - 1; pt >= 0; pt--)
            {
                if (playerTokens[pt] > 0 && pt != activePlayerIndex)
                {
                    playerTokens[pt]--;
                }
            }
            PrintPlayerTokens(players, playerTokens);

            break;
        }

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

        if (!hasValidPlay)
        {
            Console.WriteLine($"{currentPlayerName} cannot play a card and loses a token.");
            PrintPlayerHand(playerHands[activePlayerIndex]);
            playerTokens[activePlayerIndex]--;
            PrintPlayerTokens(players, playerTokens);

            break;
        }

        StandardCard selectedCard;
        StandardRank selectedCardRank;
        int cardValue;

        if (activePlayerIndex == 0)
        {
            for (int cc = 0; cc < 3; cc++)
            {
                Console.WriteLine($"\t {cc + 1} {playerHands[0][cc]}");
            }
            bool isValidCard = false;
            do
            {
                int selectedMenuCard = 0;
                do
                {
                    Console.Write($"Which card would you like to play, {currentPlayerName}? ");
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
        }
        else    // computer turn
        {
            Console.WriteLine($"{currentPlayerName}'s turn.");
            Thread.Sleep(1500);

            var playMe = playerHands[activePlayerIndex].Select(ph => new { card = ph, value = ComputerPlayValue(ph.Rank) }).OrderByDescending(v => v.value).First(v => discardTotal + v.value < 100);
            selectedCard = playMe.card;
            selectedCardRank = selectedCard.Rank;
            cardValue = playMe.card.Rank == StandardRank.Nine ? 99 : playMe.value;
        }

        Console.WriteLine($"{currentPlayerName} played {selectedCard} for " + (cardValue == 99 ? $"{cardValue - discardTotal} resulting in a discard total of 99" : $"{cardValue}"));
        discardTotal = cardValue == 99 ? 99 : discardTotal + cardValue;

        playerHands[activePlayerIndex].Remove(selectedCard);
        playerHands[activePlayerIndex].Add(drawPile.DrawCard());

        if (selectedCardRank == StandardRank.Three)
        {
            activePlayerIndex = AdvancePlay(activePlayerIndexes, activePlayerIndex, isForwardPlay);
            Console.WriteLine($"{currentPlayerName} skipped {players[activePlayerIndex]}!");
        }

        if (selectedCardRank == StandardRank.Four)
        {
            Console.WriteLine($"{currentPlayerName} reversed play!");
            isForwardPlay = !isForwardPlay;
        }

        Thread.Sleep(1500);

        activePlayerIndex = AdvancePlay(activePlayerIndexes, activePlayerIndex, isForwardPlay);
    }
}

Console.WriteLine(Environment.NewLine + $"{players[playerTokens.ToList().FindIndex(t => t > 0)]} is the winner!");
Thread.Sleep(1500);
Console.WriteLine(Environment.NewLine + $"Press any key to exit, {playerName}.");
Console.ReadKey();

static int ComputerPlayValue(StandardRank cardRank)
{
    return cardRank switch
    {
        StandardRank.Ace => 1,
        StandardRank.Ten => -10,
        StandardRank.Jack or StandardRank.Queen => 10,
        StandardRank.Four or StandardRank.King or StandardRank.Nine => 0,
        _ => (int)cardRank
    };
}

static int SelectCardValue(int[] options)
{
    int selectedValue = -99;
    do
    {
        Console.Write($"{options[0]} or {options[1]}? ");
    } while (!int.TryParse(Console.ReadLine(), out selectedValue) || !options.Contains(selectedValue));
    return selectedValue;
}

static int AdvancePlay(int[] activePlayerIndexes, int activePlayerIndex, bool isForwardPlay)
{
    // https://codereview.stackexchange.com/a/126223/150099
	int selectionIndex = Array.IndexOf(activePlayerIndexes, activePlayerIndex);
	
    if (isForwardPlay)
    {
		selectionIndex++;
    }
    else
    {
		selectionIndex--;
    }
	
	selectionIndex += activePlayerIndexes.Length;
	selectionIndex %= activePlayerIndexes.Length;

	return activePlayerIndexes[selectionIndex];
}

static bool IsMichiganWin(List<StandardCard> hand)
{
    return hand[0].Rank == hand[1].Rank && hand[0].Rank == hand[2].Rank;
}

static void PrintPlayerHand(List<StandardCard> playerHand)
{
    Console.WriteLine($"\t{playerHand[0]} - {playerHand[1]} - {playerHand[2]}" + Environment.NewLine);
}

static void PrintPlayerTokens(Dictionary<int, string> players, int[] playerTokens)
{
    Thread.Sleep(3000);
    for (int pc = 0; pc < players.Count; pc++)
    {
        Console.WriteLine($"{players[pc],-10}: " + (playerTokens[pc] > 0 ? $"{playerTokens[pc]} tokens" : "out"));
    }
    Thread.Sleep(3000);
}