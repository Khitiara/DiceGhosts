using DiceGhosts;
using NUnit.Framework.Constraints;

namespace tests;

public class ExprArithmeticTests
{
    [TestCase]
    public void TestExprTotalValue()
    {
        DiceExpr expr = new DiceExpr.SumExpr(
            new DiceExpr.RollExpr([new DiceRoller.NormalRoll(3), new DiceRoller.NormalRoll(5)]),
            new DiceExpr.BinaryExpr(DiceExpr.BinaryExpr.Operators.Multiply,
                new DiceExpr.RollExpr([new DiceRoller.ChainRoll([-1, 3])]), new DiceExpr.ConstExpr(3)));
        Assert.That(() => expr.TotalValue, new EqualConstraint(17));
    }
}