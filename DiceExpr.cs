using System.Text;

namespace DiceGhosts;

public abstract partial record DiceExpr
{
    public abstract void PrintRoot(StringBuilder builder);

    public abstract int TotalValue { get; }

    public virtual void Print(StringBuilder builder)
    {
        builder.Append("(");
        PrintRoot(builder);
        builder.Append(")");
    }

    public record ConstExpr(int Value) : DiceExpr
    {
        public override void PrintRoot(StringBuilder builder)
        {
            builder.Append(Value.ToString());
        }

        public override int TotalValue => Value;

        public override void Print(StringBuilder builder)
        {
            PrintRoot(builder);
        }
    }

    public record RollExpr(DiceRoller.RollSegment[] Rolls) : DiceExpr
    {
        public override void PrintRoot(StringBuilder builder)
        {
            builder.Append("[[");

            foreach (DiceRoller.RollSegment roll in Rolls[..^1]) {
                roll.Write(builder);
                builder.Append(", ");
            }

            Rolls[^1].Write(builder);
            builder.Append($"]] {Rolls.Sum(r => r.ContributedValue)}");
        }

        public override int TotalValue => Rolls.Sum(r => r.ContributedValue);
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

        public override void PrintRoot(StringBuilder builder)
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

        public override int TotalValue => Operator switch {
            Operators.Add => Left.TotalValue + Right.TotalValue,
            Operators.Subtract => Left.TotalValue - Right.TotalValue,
            Operators.Multiply => Left.TotalValue * Right.TotalValue,
            Operators.Divide => Left.TotalValue / Right.TotalValue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}