namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;

    /// <summary>
    /// Implements a battleship player that shoots at one cell after the other
    /// </summary>
    public class Sequential : PlayerBase
    {
        /// <inheritdoc />
        public override void GetShot(Guid _, string __, IReadOnlyBoard board, Shoot shoot)
        {
            var ix = new BoardIndex();

            // Find next unknown square
            while (board[ix] != SquareContent.Unknown) ix = ix.Next();

            // Shoot at first unknonwn square
            shoot(ix);
        }
    }
}
