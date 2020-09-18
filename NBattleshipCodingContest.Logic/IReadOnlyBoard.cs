namespace NBattleshipCodingContest.Logic
{
    using System.Collections.Generic;

    public interface IReadOnlyBoard : IEnumerable<SquareContent>, IReadOnlyList<SquareContent>
    {
        /// <summary>
        /// Gets or sets the content on a given board square.
        /// </summary>
        /// <param name="ix">Index of the square</param>
        /// <returns>Content of the given square</returns>
        SquareContent this[BoardIndex ix] { get; }
    }
}
