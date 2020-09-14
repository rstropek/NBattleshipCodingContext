namespace NBattleshipCodingContest.Logic.Tests
{
    using Moq;
    using Xunit;

    public class RandomBoardFillterTests
    {
        [Fact]
        public void Fill()
        {
            var boardMock = new Mock<IFillableBoard>();
            boardMock.Setup(b => b.TryPlaceShip(It.IsAny<BoardIndex>(), It.IsAny<int>(), It.IsAny<Direction>()))
                .Returns(true);

            var rbf = new RandomBoardFiller();
            rbf.Fill(new[] { 2, 3, 4 }, boardMock.Object);

            // Verify that ships have been placed
            boardMock.Verify(b => b.TryPlaceShip(It.IsAny<BoardIndex>(), It.IsAny<int>(), It.IsAny<Direction>()),
                Times.Exactly(3));
        }

        [Fact]
        public void Fill_Failure()
        {
            var boardMock = new Mock<IFillableBoard>();
            boardMock.Setup(b => b.TryPlaceShip(It.IsAny<BoardIndex>(), It.IsAny<int>(), It.IsAny<Direction>()))
                .Returns(false);

            var rbf = new RandomBoardFiller();
            Assert.Throws<BoardTooOccupiedException>(() => rbf.Fill(new[] { 2 }, boardMock.Object));

            // Verify that no ship has been placed
            boardMock.Verify(b => b.TryPlaceShip(It.IsAny<BoardIndex>(), It.IsAny<int>(), It.IsAny<Direction>()),
                Times.Exactly(1000));
        }
    }
}
