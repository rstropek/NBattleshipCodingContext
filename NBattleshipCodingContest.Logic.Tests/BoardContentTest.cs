namespace NBattleshipCodingContest.Logic.Tests
{
    using System.Linq;
    using Xunit;

    public class BoardContentTest
    {
        [Fact]
        public void BoardContentToString()
        {
            var bc = new BoardContent();
            bc.Clear(SquareContent.Unknown);
            bc[new BoardIndex(0, 0)] = SquareContent.Ship;
            bc[new BoardIndex(1, 0)] = SquareContent.HitShip;
            bc[new BoardIndex(2, 0)] = SquareContent.Water;

            var output = bc.ToString();
            Assert.Equal((1 + 10 + 9 + 1) * (1 + 10 * 3 + 1), output.Length);
            Assert.Equal(2, output.Count(c => c == '█'));
            Assert.Equal(2, output.Count(c => c == 'X'));
            Assert.Equal(2, output.Count(c => c == '~'));
            Assert.Equal(97 * 2, output.Count(c => c == ' '));
        }
    }
}
