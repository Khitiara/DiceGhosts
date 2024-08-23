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
}