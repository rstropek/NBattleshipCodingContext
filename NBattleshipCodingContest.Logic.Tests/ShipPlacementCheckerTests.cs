namespace NBattleshipCodingContest.Logic.Tests
{
    using System;
    using Xunit;
    using static NBattleshipCodingContest.Logic.ShipPlacementChecker;

    public class ShipPlacementCheckerTests
    {
        [Fact]
        public void CanPlaceShip_Invalid_Col() =>
            Assert.Throws<ArgumentException>("col", () => CanPlaceShip(10, 0, 2, Direction.Horizontal, (_, _) => false));

        [Fact]
        public void CanPlaceShip_Invalid_Row() =>
            Assert.Throws<ArgumentException>("row", () => CanPlaceShip(0, 10, 2, Direction.Horizontal, (_, _) => false));

        [Fact]
        public void CanPlaceShip_Invalid_Direction() =>
            Assert.Throws<ArgumentException>("direction", () => CanPlaceShip(0, 0, 2, (Direction)99, (_, _) => false));

        [Fact]
        public void CanPlaceShip_Outside_Bounds_Horizontal() =>
            Assert.False(CanPlaceShip(9, 0, 2, Direction.Horizontal, (_, _) => true));

        [Fact]
        public void CanPlaceShip_Outside_Bounds_Vertical() =>
            Assert.False(CanPlaceShip(0, 9, 2, Direction.Vertical, (_, _) => true));

        [Theory]
        [InlineData(0, 0, Direction.Horizontal, 0, 2, 0, 1)]    // Left upper corner
        [InlineData(0, 0, Direction.Vertical, 0, 1, 0, 2)]
        [InlineData(8, 9, Direction.Horizontal, 7, 9, 8, 9)]    // Right lower corner
        [InlineData(9, 8, Direction.Vertical, 8, 9, 7, 9)]
        [InlineData(0, 1, Direction.Horizontal, 0, 2, 0, 3)]    // Left border
        [InlineData(0, 1, Direction.Vertical, 0, 1, 0, 3)]
        [InlineData(8, 1, Direction.Horizontal, 7, 9, 0, 2)]    // Right border
        [InlineData(1, 8, Direction.Vertical, 0, 2, 7, 9)]
        [InlineData(1, 1, Direction.Horizontal, 0, 3, 0, 3)]    // Middle (no border)
        [InlineData(1, 1, Direction.Vertical, 0, 3, 0, 3)]
        public void CanPlaceShip_Overlap_Coordinates_FirstRow_Horizontal(int col, int row, Direction direction, int minCol, int maxCol, int minRow, int maxRow)
        {
            CanPlaceShip(col, row, 2, direction, (c, r) =>
            {
                Assert.InRange(c, minCol, maxCol);
                Assert.InRange(r, minRow, maxRow);
                return false;
            });
        }
    }
}