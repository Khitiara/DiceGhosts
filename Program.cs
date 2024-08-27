using System.Buffers;
using System.Text;
using DiceGhosts;
using Spectre.Console;

AnsiConsole.WriteLine("Dice Ghosts Dice Roller v1");

SearchValues<char> search = SearchValues.Create(" \t");
StringBuilder stringBuilder = new();

while (true) {
    string command = AnsiConsole.Prompt(new TextPrompt<string>("[bold]>[/] "));
    int split = command.AsSpan().IndexOfAny(search);
    ReadOnlySpan<char> cmd = split == -1 ? command : command.AsSpan(..split);
    switch (cmd) {
        case "x":
            return;
        case "r":
            if (split == -1) {
                AnsiConsole.WriteLine("Cannot roll nothing");
                continue;
            }
            string rest = command[(split + 1)..];
            DiceExpr result = DiceExpr.ParseAndRoll(Random.Shared, rest);
            stringBuilder.Clear();
            result.PrintRoot(stringBuilder);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($" = {result.TotalValue}");
            AnsiConsole.Markup(stringBuilder.ToString());
            break;
    }
}