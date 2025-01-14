﻿// See https://aka.ms/new-console-template for more information
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
    int activePlayerIndex = 0;
    // round loop
    do
    {
        if (playerTokens[activePlayerIndex] == 0)
        {
            continue;
        }

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
        }
        else
        {
            Console.WriteLine($"{players[activePlayerIndex]}'s turn.");
        }

        activePlayerIndex++;
        /*********************************
        **                              **
        **  HANDLE UNSTABLE CODE STATE  **
        **                              **
        *********************************/
        endRund = activePlayerIndex == 3;
        /*********************************
        **                              **
        **  HANDLE UNSTABLE CODE STATE  **
        **                              **
        *********************************/
        if (activePlayerIndex == players.Count)
        {
            activePlayerIndex = 0;
        }
    } while (!endRund);

    /*********************************
    **                              **
    **  HANDLE UNSTABLE CODE STATE  **
    **                              **
    *********************************/
    break;
    /*********************************
    **                              **
    **  HANDLE UNSTABLE CODE STATE  **
    **                              **
    *********************************/
} while (playerTokens.Count(t => t > 0) > 1);

Console.WriteLine($"Press any key to exit, {playerName}.");
Console.ReadKey();