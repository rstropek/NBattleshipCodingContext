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
    public abstract class BoardContent : IReadOnlyBoard
    {
        private readonly SquareContent[] boardContent = new SquareContent[10 * 10];

        protected void Clear(SquareContent content)
        {
            for (var i = 0; i < 10 * 10; i++)
            {
                boardContent[i] = content;
            }
        }

        /// <summary>
        /// Gets the content on a given board square.
        /// </summary>
        /// <param name="col">Zero-based column index</param>
        /// <param name="row">Zero-based row index</param>
        /// <returns>Content of the given square</returns>
        /// <exception cref="System.ArgumentException">Thrown in case of invalid parameters</exception>
        public SquareContent this[BoardIndex ix]
        {
            get => this[ix];
            set => this[ix] = value;
        }

        /// <inheritdoc/>
        public int Count => 10 * 10;

        private void VerifyLocation(int location)
        {
            if (location is < 0 or >= 10 * 10)
            {
                throw new ArgumentOutOfRangeException(nameof(location), "Invalid location, must be between 0 and 99");
            }
        }

        /// <inheritdoc/>
        public SquareContent this[int location]
        {
            get
            {
                VerifyLocation(location);
                return boardContent[location];
            }

            set
            {
                VerifyLocation(location);
                if (value == SquareContent.Unknown)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set square content to unknown");
                }

                boardContent[location] = value;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<SquareContent> GetEnumerator()
        {
            foreach (var square in boardContent)
            {
                yield return square;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
