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
        case "exit":
        case "q":
        case "quit":
            return;
        case "r":
        case "roll":
        case "c":
        case "calc":
            if (split == -1) {
                AnsiConsole.MarkupLine("[yellow]Cannot roll nothing[/]");
                continue;
            }

            string rest = command[(split + 1)..];
            DiceExpr result = DiceExpr.ParseAndRoll(Random.Shared, rest);
            stringBuilder.Clear();
            stringBuilder.Append("Rolled ");
            result.PrintRoot(stringBuilder);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($" = {result.TotalValue}");
            AnsiConsole.Markup(stringBuilder.ToString());
            break;
        default:
            AnsiConsole.MarkupLineInterpolated($"[red]Unknown command:[/] \"{command}\"");
            break;
    }
}