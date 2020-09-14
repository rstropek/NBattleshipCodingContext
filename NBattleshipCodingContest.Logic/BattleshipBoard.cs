namespace NBattleshipCodingContest.Logic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static NBattleshipCodingContest.Logic.ShipPlacementChecker;

    /// <summary>
    /// <see cref="BattleshipBoard"/> implements a board for a battleship game.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The rules of the classical Battleship game apply (see also
    /// https://en.wikipedia.org/wiki/Battleship_(game)).
    /// </para>
    /// <para>
    /// Methods reading data are thread-safe, methods writing data are not.
    /// </para>
    /// </remarks>
    public class BattleshipBoard : IFillableBoard, IReadOnlyBoard
    {
        private readonly SquareContent[] BoardContent = new SquareContent[10 * 10];

        internal void PlaceShip(BoardIndex ix, int shipLength, Direction direction)
        {
            for (var i = 0; i < shipLength; i++)
            {
                BoardContent[ix] = SquareContent.Ship;
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

        private void Clear(SquareContent content = SquareContent.Water)
        {
            for (var i = 0; i < 10 * 10; i++)
            {
                BoardContent[i] = content;
            }
        }

        /// <summary>
        /// Gets the content on a given board square.
        /// </summary>
        /// <param name="col">Zero-based column index</param>
        /// <param name="row">Zero-based row index</param>
        /// <returns>Content of the given square</returns>
        /// <exception cref="System.ArgumentException">Thrown in case of invalid parameters</exception>
        public SquareContent this[BoardIndex ix] => BoardContent[ix];

        /// <inheritdoc/>
        public int Count => 10 * 10;

        /// <inheritdoc/>
        public SquareContent this[int location]
        {
            get
            {
                if (location is < 0 or >= 10 * 10)
                {
                    throw new ArgumentException("Invalid location, must be between 0 and 99", nameof(location));
                }

                return BoardContent[location];
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
            Clear();
            filler.Fill(new[] { 5, 4, 3, 3, 2 }, this);
        }

        /// <summary>
        /// Initializes the board with all squares unknown.
        /// </summary>
        public void Initialize()
        {
            Clear(SquareContent.Unknown);
        }

        /// <inheritdoc/>
        public bool TryPlaceShip(BoardIndex ix, int shipLength, Direction direction)
        {
            if (!CanPlaceShip(ix, shipLength, direction,
                (c, r) => BoardContent[ix] == SquareContent.Water))
            {
                return false;
            }

            PlaceShip(ix, shipLength, direction);

            return true;
        }

        /// <inheritdoc/>
        public IEnumerator<SquareContent> GetEnumerator()
        {
            foreach (var square in BoardContent)
            {
                yield return square;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Places a shot at the given coordinates.
        /// </summary>
        /// <param name="col">Zero-based index of column</param>
        /// <param name="row">Zero-based index of row</param>
        /// <returns>
        /// New content of given board square (<see cref="SquareContent.Water"/>
        /// or <see cref="SquareContent.HitShip"/>).
        /// </returns>
        public SquareContent ShootAt(BoardIndex ix) => ShootAt(ix);

        private SquareContent ShootAt(int ix)
        {
            // Note switch expression

            var c = BoardContent[ix];
            return c switch
            {
                SquareContent.Ship => BoardContent[ix] = SquareContent.HitShip,
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
