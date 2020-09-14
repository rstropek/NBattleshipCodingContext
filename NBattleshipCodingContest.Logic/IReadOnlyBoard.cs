namespace NBattleshipCodingContest.Logic
{
    using System.Collections.Generic;

    public interface IReadOnlyBoard : IEnumerable<SquareContent>, IReadOnlyList<SquareContent>
    {
        /// <summary>
        /// Gets the content on a given board square.
        /// </summary>
        /// <param name="col">Zero-based column index</param>
        /// <param name="row">Zero-based row index</param>
        /// <returns>Content of the given square</returns>
        /// <exception cref="System.ArgumentException">Thrown in case of invalid parameters</exception>
        SquareContent this[BoardIndex ix] { get; }
    }
}
