// See https://aka.ms/new-console-template for more information
Console.WriteLine("Ninety Nine - A Card Game");

string playerName;
do
{
    Console.Write("What is your name? ");
    playerName = Console.ReadLine()?.Trim() ?? String.Empty;
} while (String.IsNullOrEmpty(playerName));
Console.WriteLine(Environment.NewLine + $"Hello, {playerName}.");

Console.WriteLine($"Press any key to exit, {playerName}.");
Console.ReadKey();