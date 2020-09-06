namespace NBattleshipCodingContest.Logic.Tests
{
    using System.Linq;
    using Xunit;

    public class BattleshipBoardTests
    {
        [Fact] 
        public void PlaceShip_Horizontal()
        {
            var board = new BattleshipBoard();
            board.PlaceShip(0, 0, 2, Direction.Horizontal);
            Assert.Equal(SquareContent.Ship, board[0, 0]);
            Assert.Equal(SquareContent.Ship, board[1, 0]);
            Assert.Equal(2, board.Count(s => s != SquareContent.Water));
        }

        [Fact]
        public void PlaceShip_Vertical()
        {
            var board = new BattleshipBoard();
            board.PlaceShip(0, 0, 2, Direction.Vertical);
            Assert.Equal(SquareContent.Ship, board[0, 0]);
            Assert.Equal(SquareContent.Ship, board[0, 1]);
            Assert.Equal(2, board.Count(s => s != SquareContent.Water));
        }

        [Fact]
        public void Initialize()
        {
            var board = new BattleshipBoard();
            board.Initialize(new RandomBoardFiller());
            Assert.Equal(5 + 4 + 3 + 3 + 2, board.Count(s => s != SquareContent.Water));
        }

        [Fact]
        public void ShootAt()
        {
            var board = new BattleshipBoard();
            board.PlaceShip(0, 0, 2, Direction.Horizontal);
            board.ShootAt(0, 0);
            Assert.Equal(SquareContent.HitShip, board[0, 0]);
            Assert.Equal(SquareContent.Ship, board[1, 0]);
        }

        [Fact]
        public void HasLost()
        {
            var board = new BattleshipBoard();
            board.PlaceShip(0, 0, 1, Direction.Horizontal);
            board.ShootAt(0, 0);
            Assert.True(board.HasLost);
        }
    }
}
