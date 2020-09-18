namespace NBattleshipCodingContest.Logic
{
    using System.Diagnostics;
    using System.Linq;
    using static NBattleshipCodingContest.Logic.ShipPlacementChecker;

    /// <summary>
    /// <see cref="BattleshipBoard"/> implements a board for a battleship game.
    /// </summary>
    /// <remarks>
    /// The rules of the classical Battleship game apply (see also
    /// https://en.wikipedia.org/wiki/Battleship_(game)).
    /// </remarks>
    public class BattleshipBoard : BoardContent, IFillableBoard
    {
        internal void PlaceShip(BoardIndex ix, int shipLength, Direction direction)
        {
            for (var i = 0; i < shipLength; i++)
            {
                this[ix] = SquareContent.Ship;
                if (i != shipLength - 1)
                {
                    if (direction == Direction.Horizontal)
                    {
                        ix = ix.NextColumn();
                    }
                    else
                    {
                        ix = ix.NextRow();
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the board by using the given filler.
        /// </summary>
        /// <param name="filler">Filler to use for filling the board</param>
        /// <remarks>
        /// The board (i.e., everything is set to water) is cleared before filling it.
        /// </remarks>
        public void Initialize(IBoardFiller filler)
        {
            Clear(SquareContent.Water);
            filler.Fill(new[] { 5, 4, 3, 3, 2 }, this);
        }

        /// <summary>
        /// Initializes the board with all squares unknown.
        /// </summary>
        public void Initialize() => Clear(SquareContent.Unknown);

        /// <inheritdoc/>
        public bool TryPlaceShip(BoardIndex ix, int shipLength, Direction direction)
        {
            if (!CanPlaceShip(ix, shipLength, direction, (c, r) => this[new BoardIndex(c, r)] == SquareContent.Water))
            {
                return false;
            }

            PlaceShip(ix, shipLength, direction);

            return true;
        }

        /// <summary>
        /// Places a shot at the given coordinates.
        /// </summary>
        /// <param name="col">Zero-based index of column</param>
        /// <param name="row">Zero-based index of row</param>
        /// <returns>
        /// New content of given board square (<see cref="SquareContent.Water"/>
        /// or <see cref="SquareContent.HitShip"/>).
        /// </returns>
        public SquareContent ShootAt(BoardIndex ix) => ShootAt((int)ix);

        private SquareContent ShootAt(int ix)
        {
            // Note switch expression

            var c = this[ix];
            return c switch
            {
                SquareContent.Ship => this[ix] = SquareContent.HitShip,
                _ => c,
            };
        }

        /// <summary>
        /// Indicates if the player of this board has lost.
        /// </summary>
        /// <remarks>
        /// Lost means that all ship squares were hit by shots.
        /// </remarks>
        /// <seealso cref="ShootAt(int, int)"/>
        public bool HasLost => !this.Any(s => s == SquareContent.Ship) && this.Any(s => s == SquareContent.HitShip);
    }
}
