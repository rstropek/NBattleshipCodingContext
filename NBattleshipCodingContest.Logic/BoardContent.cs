namespace NBattleshipCodingContest.Logic
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="BattleshipBoard"/> implements a board for a battleship game.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Methods reading data are thread-safe, methods writing data are not.
    /// </para>
    /// </remarks>
    public abstract class BoardContent : IReadOnlyBoard
    {
        private readonly SquareContent[] boardContent = new SquareContent[10 * 10];

        /// <summary>
        /// Set all squares of the board to a given content
        /// </summary>
        /// <param name="content">Square content that should be written to all squares</param>
        protected void Clear(SquareContent content)
        {
            for (var i = 0; i < 10 * 10; i++)
            {
                boardContent[i] = content;
            }
        }

        /// <inheritdoc/>
        public SquareContent this[BoardIndex ix]
        {
            get => boardContent[ix];
            set => boardContent[ix] = value;
        }

        /// <inheritdoc/>
        public int Count => 10 * 10;

        /// <inheritdoc/>
        public SquareContent this[int location]
        {
            get => this[new BoardIndex(location)];
            set => this[new BoardIndex(location)] = value;
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
