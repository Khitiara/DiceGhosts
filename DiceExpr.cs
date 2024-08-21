using Spectre.Console;

namespace DiceGhosts;

public abstract partial record DiceExpr
{
    public abstract void PrintRoot(Paragraph builder);

    public virtual void Print(Paragraph builder)
    {
        builder.Append("(");
        PrintRoot(builder);
        builder.Append(")");
    }

    public record ConstExpr(int Value) : DiceExpr
    {
        public override void PrintRoot(Paragraph builder)
        {
            builder.Append(Value.ToString());
        }

        public override void Print(Paragraph builder)
        {
            PrintRoot(builder);
        }
    }

    public record RollExpr(DiceRoller.RollSegment[] Rolls) : DiceExpr
    {
        public override void PrintRoot(Paragraph builder)
        {
            builder.Append("[");

            foreach (DiceRoller.RollSegment roll in Rolls[..^1]) {
                roll.Write(builder);
                builder.Append(", ");
            }

            Rolls[^1].Write(builder);
            builder.Append($"] {Rolls.Sum(r => r.ContributedValue)}");
        }
    }

    public record BinaryExpr(BinaryExpr.Operators Operator, DiceExpr Left, DiceExpr Right) : DiceExpr
    {
        public enum Operators
        {
            Add,
            Subtract,
            Multiply,
            Divide,
        }

        public override void PrintRoot(Paragraph builder)
        {
            Left.Print(builder);
            builder.Append(" ");
            builder.Append(Operator switch {
                Operators.Add => "+",
                Operators.Subtract => "-",
                Operators.Multiply => "*",
                Operators.Divide => "/",
                _ => throw new ArgumentOutOfRangeException()
            });
            builder.Append(" ");
            Right.Print(builder);
        }
    }
}