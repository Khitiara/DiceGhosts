using DiceGhosts;

namespace tests;

[TestFixture]
public class RollingTest
{
    [TestCase(0, 8, 6, 0, 0, 0, 6, 1)]
    [TestCase(1, 2, 10, 0, 2, -1, 0, 0)]
    [TestCase(0, 2, 20, 1, 0, 0, 0, 0)]
    [TestCase(1, 2, 10, 0, 2, -1, 5, -1)]
    public Task TestRolls(int seed, int count, int sides, int keep, int rerollMax,
        int rerollCount, int explodeOn, int explodeCount)
    {
        Random rand = new(seed);
        return Verify(DiceRoller.Roll(rand, count, sides, keep, rerollMax, rerollCount, explodeOn, explodeCount))
            .UseDirectory("Verify");
    }
}