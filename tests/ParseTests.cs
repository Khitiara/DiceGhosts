using DiceGhosts;

namespace tests;

public class ParseTests
{
    [TestCase("1d4 + 3")]
    [TestCase("8d6r1.-1!8.1")]
    [TestCase("2d20k1")]
    public Task TestExprParse(string text)
    {
        return Verify(DiceExpr.Parser.ParseExpr(text))
            .UseDirectory("Verify");
    }

    [TestCase(0, "1d4 + 3")]
    [TestCase(1, "8d6r1.-1!6.1")]
    [TestCase(0, "2d20k1")]
    public Task TestExprParseExecute(int seed, string text)
    {
        return Verify(DiceExpr.ParseAndRoll(new Random(seed), text))
            .UseDirectory("Verify");
    }
}