using Sprache;

namespace DiceGhosts;

public abstract partial record DiceExpr
{
    internal static class Parser
    {
        private static Parser<BinaryExpr.Operators> Operator(string op, BinaryExpr.Operators opType) =>
            Parse.String(op).Token().Return(opType);

        private static readonly Parser<BinaryExpr.Operators> Add = Operator("+", BinaryExpr.Operators.Add);
        private static readonly Parser<BinaryExpr.Operators> Sub = Operator("-", BinaryExpr.Operators.Subtract);
        private static readonly Parser<BinaryExpr.Operators> Mul = Operator("*", BinaryExpr.Operators.Multiply);
        private static readonly Parser<BinaryExpr.Operators> Div = Operator("/", BinaryExpr.Operators.Divide);

        private static readonly Parser<int> UInt = Parse.Digit.AtLeastOnce().Text().Select(int.Parse);

        private static readonly Parser<int> Int =
            from neg in Parse.Char('-').Once().Optional()
            from i in UInt
            select neg.IsDefined ? -i : i;

        private static readonly Parser<(int, int)?> Reroll =
            (from r in Parse.Char('r')
                from max in UInt
                from dot in Parse.Char('.')
                from count in Int
                select new (int, int)?((max, count))).Optional().Select(o => o.GetOrElse(null));

        private static readonly Parser<(int, int)?> Explode =
            (from r in Parse.Char('!')
                from min in UInt
                from dot in Parse.Char('.')
                from count in Int
                select new (int, int)?((min, count))).Optional().Select(o => o.GetOrElse(null));

        private static readonly Parser<RollArgsInner> KeepOrRerollExplode =
            (from k in Parse.Char('k')
                from num in Int
                select (RollArgsInner)new KeepArgs(num))
            .XOr(from reroll in Reroll
                from explode in Explode
                select new RerollArgs(reroll, explode));

        private static readonly Parser<PreRollExpr> Roll =
            from count in UInt
            from d in Parse.Char('d')
            from sides in UInt
            from extras in KeepOrRerollExplode
            select new RollArgs(count, sides, extras);

        private static readonly Parser<PreRollExpr> Operand =
            (from lparen in Parse.Char('(')
                from expr in Parse.Ref(() => Expr)
                from rparen in Parse.Char(')')
                select expr)
            .XOr(Int.Select(PreRollExpr.MakeConst));

        private static readonly Parser<PreRollExpr> Term = Roll.Or(Operand).Token();

        private static readonly Parser<PreRollExpr> MulDiv =
            Parse.XChainOperator(Mul.XOr(Div), Term.Token(), PreRollExpr.MakeBinary);

        private static readonly Parser<PreRollExpr> Expr =
            Parse.XChainOperator(Add.XOr(Sub), MulDiv.Token(), PreRollExpr.MakeBinary);

        internal static PreRollExpr ParseExpr(string text) => Expr.End().Parse(text);

        internal abstract record PreRollExpr
        {
            public static PreRollExpr MakeBinary(BinaryExpr.Operators op, PreRollExpr left, PreRollExpr right) =>
                new Binary(op, left, right);

            public static PreRollExpr MakeConst(int value) => new Const(value);

            public abstract DiceExpr Execute(Random random);
        }

        internal record Const(int Value) : PreRollExpr
        {
            public override DiceExpr Execute(Random random)
            {
                return new ConstExpr(Value);
            }
        }

        internal record Binary(BinaryExpr.Operators Op, PreRollExpr Left, PreRollExpr Right) : PreRollExpr
        {
            public override DiceExpr Execute(Random random)
            {
                return new BinaryExpr(Op, Left.Execute(random), Right.Execute(random));
            }
        }

        internal record RollArgs(int Count, int Sides, RollArgsInner Inner) : PreRollExpr
        {
            public override DiceExpr Execute(Random random)
            {
                int keep = 0, rerollMax = 0, rerollCount = 0, explodeMin = 0, explodeCount = 0;
                switch (Inner) {
                    case KeepArgs keepArgs:
                        keep = keepArgs.Keep;
                        break;
                    case RerollArgs rerollArgs:
                        if (rerollArgs.Reroll is { } r) {
                            rerollMax = r.Item1;
                            rerollCount = r.Item2;
                        }

                        if (rerollArgs.Explode is { } e) {
                            explodeMin = e.Item1;
                            explodeCount = e.Item2;
                        }

                        break;
                }

                return new RollExpr(DiceRoller.Roll(random, Count, Sides, keep, rerollMax, rerollCount, explodeMin,
                    explodeCount));
            }
        }

        internal abstract record RollArgsInner;

        internal record KeepArgs(int Keep) : RollArgsInner;

        internal record RerollArgs((int, int)? Reroll, (int, int)? Explode) : RollArgsInner;
    }

    public static DiceExpr ParseAndRoll(Random random, string text)
    {
        return Parser.ParseExpr(text).Execute(random);
    }
}