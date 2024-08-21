using Spectre.Console;

namespace DiceGhosts;

public static class DiceRoller
{
    public abstract record RollSegment
    {
        public abstract int ContributedValue { get; }
        public abstract void Write(Paragraph writer);
    }

    public record NormalRoll(int Roll) : RollSegment
    {
        public override int ContributedValue => Roll;

        public override void Write(Paragraph writer)
        {
            writer.Append(Roll.ToString());
        }
    }

    public record DiscardedRoll(int Roll) : RollSegment
    {
        public override int ContributedValue => 0;

        public override void Write(Paragraph writer)
        {
            writer.Append(Roll.ToString(), new Style(decoration: Decoration.Strikethrough));
        }
    }

    public record ChainRoll(int[] Rolls) : RollSegment
    {
        public override int ContributedValue => Rolls.Where(i => i > 0).Sum();

        public override void Write(Paragraph writer)
        {
            foreach (int i in Rolls[..^1]) {
                if (i < 0) {
                    writer.Append((-i).ToString(), new Style(decoration: Decoration.Strikethrough));
                } else {
                    writer.Append($"{i.ToString()}!", new Style(decoration: Decoration.Bold));
                }

                writer.Append(" ");
            }

            writer.Append(Rolls[^1].ToString());
        }
    }

    private static int[] RollChain(Random random, int sides, int roll, int rerollMax, int explodeMin, int rerolls,
        int explodes)
    {
        List<int> rolls = new(rerolls + explodes) { roll };
        while (true) {
            if (rerolls != 0 && roll <= rerollMax) {
                rolls[^1] = -rolls[^1]; // mark to discard
                rolls.Add(roll = random.Next(sides) + 1);
                rerolls--;
            } else if (explodes != 0 && roll >= explodeMin) {
                rolls.Add(roll = random.Next(sides) + 1);
                explodes--;
            } else {
                break;
            }
        }

        return rolls.ToArray();
    }

    public static RollSegment[] Roll(Random random, int count, int sides, int keep, int rerollMax,
        int rerollCount, int explodeOn, int explodeCount)
    {
        int[] choices = Enumerable.Range(1, sides).ToArray();
        (int value, int index)[] rolls = random.GetItems(choices, count)
            .Select((value, index) => (value, index))
            .OrderBy(p => p.value).ToArray();
        RollSegment[] output = new RollSegment[rolls.Length];
        switch (keep) {
            case < 0: {
                // keep lowest
                for (int i = 0; i < -keep; i++) {
                    output[rolls[i].index] = new NormalRoll(rolls[i].value);
                }

                for (int i = -keep; i < rolls.Length; i++) {
                    output[rolls[i].index] = new DiscardedRoll(rolls[i].value);
                }

                break;
            }
            case > 0: {
                for (int i = 1; i <= keep; i++) {
                    output[rolls[^i].index] = new NormalRoll(rolls[^i].value);
                }

                for (int i = keep + 1; i <= rolls.Length; i++) {
                    output[rolls[^i].index] = new DiscardedRoll(rolls[^i].value);
                }

                break;
            }
            default: {
                if (rerollCount != 0 || explodeCount != 0) {
                    foreach ((int value, int index) in rolls) {
                        output[index] = new ChainRoll(RollChain(random, sides, value, rerollMax, explodeOn, rerollCount,
                            explodeCount));
                    }
                } else {
                    foreach ((int value, int index) in rolls) {
                        output[index] = new NormalRoll(value);
                    }
                }

                break;
            }
        }

        return output;
    }
}