using System.Text;
using DiceGhosts;

namespace tests;

public class FormatTests
{
    public static DiceRoller.RollSegment[] SegmentsToFormat = [
        new DiceRoller.DiscardedRoll(1),
        new DiceRoller.NormalRoll(5),
        new DiceRoller.ChainRoll([-1, 8, -2, 8, 5])
    ];

    [TestCaseSource(nameof(SegmentsToFormat))]
    public Task TestRollSegmentFormat(DiceRoller.RollSegment segment)
    {
        StringBuilder b = new();
        segment.Write(b);
        return Verify(b.ToString())
            .UseDirectory("Verify");
    }

    public static DiceExpr[] ExprsToFormat = [
        new DiceExpr.RollExpr([new DiceRoller.ChainRoll([-1, 8, 3]), new DiceRoller.ChainRoll([5])]),
    ];

    [TestCaseSource(nameof(ExprsToFormat))]
    public Task TestExprFormatSimple(DiceExpr expr)
    {
        StringBuilder b = new();
        expr.PrintRoot(b);
        return Verify(b.ToString())
            .UseDirectory("Verify");
    }

    [TestCase]
    public Task TestExprFormatComplex()
    {
        DiceExpr expr = new DiceExpr.BinaryExpr(DiceExpr.BinaryExpr.Operators.Add,
            new DiceExpr.RollExpr([new DiceRoller.NormalRoll(3), new DiceRoller.NormalRoll(5)]),
            new DiceExpr.BinaryExpr(DiceExpr.BinaryExpr.Operators.Multiply,
                new DiceExpr.RollExpr([new DiceRoller.ChainRoll([-1, 3])]), new DiceExpr.ConstExpr(3)));
        StringBuilder b = new();
        expr.PrintRoot(b);
        return Verify(b.ToString())
            .UseDirectory("Verify");
    }

    [TestCase]
    public Task TestExprFormatSum()
    {
        DiceExpr expr =
            new DiceExpr.SumExpr(new DiceExpr.RollExpr([new DiceRoller.NormalRoll(3), new DiceRoller.NormalRoll(5)]),
                new DiceExpr.RollExpr([new DiceRoller.ChainRoll([-1, 7])]), new DiceExpr.ConstExpr(2));
        StringBuilder b = new();
        expr.PrintRoot(b);
        return Verify(b.ToString())
            .UseDirectory("Verify");
    }
}