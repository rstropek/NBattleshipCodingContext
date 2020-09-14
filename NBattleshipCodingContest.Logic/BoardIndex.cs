namespace NBattleshipCodingContest.Logic
{
    using System;
    using System.Diagnostics;

    // Note readonly struct here. Read more at
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/struct#readonly-struct

    /// <summary>
    /// Index in a battleship board
    /// </summary>
    /// <remarks>
    /// <para>Contains helper methods useful when implementing the battleship board as a continuous array.</para>
    /// <para>The implementation assumes the classic battleship game with a side length of 10.</para>
    /// </remarks>
    public readonly struct BoardIndex : IEquatable<BoardIndex>
    {
        private readonly int index;

        #region Internal helper functions
        private static int GetIndex(int col, int row)
        {
            // This is a private method, so we can assume that col and row
            // contain valid values. We check them only in debug builds.

            // Read more about assertation in C#
            // https://docs.microsoft.com/en-us/visualstudio/debugger/assertions-in-managed-code
            Debug.Assert(col is >= 0 and <= 9 || row is >= 0 and <= 9);

            return row * 10 + col;
        }
        #endregion

        #region Constructors
        // Note throw expression. Read more at
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression

        // Note expression-bodied constructor. Read more at
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members#constructors

        // Note new C# 9 relational pattern matching
        // https://devblogs.microsoft.com/dotnet/welcome-to-c-9-0/#relational-patterns

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardIndex"/> type.
        /// </summary>
        /// <param name="index">Zero-based board index between 0 and 9</param>
        /// <exception cref="ArgumentOutOfRangeException">Given index is out of range</exception>
        public BoardIndex(int index) => 
            this.index = (index is >= 0 and < 100) ? index : throw new ArgumentOutOfRangeException(nameof(index));

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardIndex"/> type.
        /// </summary>
        /// <param name="col">Column index between A and J</param>
        /// <param name="row">One-based row index between 1 and 10</param>
        /// <exception cref="ArgumentOutOfRangeException">At least one of the given indexes is out of range</exception>
        public BoardIndex(char col, int row)
        {
            #region Check input parameter
            // This is a public method, so we have to check that parameters
            // contain valid values.

            if (col is < 'A' or > 'J')
            {
                throw new ArgumentOutOfRangeException(nameof(col), "Must be between A and J");
            }

            if (row is < 1 or > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(row), "Must be between 1 and 10");
            }
            #endregion

            index = GetIndex(col - 'A', row - 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardIndex"/> type.
        /// </summary>
        /// <param name="col">Zero-based column index between 0 and 9</param>
        /// <param name="row">Zero-based row index between 0 and 9</param>
        /// <exception cref="ArgumentOutOfRangeException">At least one of the given indexes is out of range</exception>
        public BoardIndex(int col, int row)
        {
            #region Check input parameter
            // This is a public method, so we have to check that parameters
            // contain valid values.

            if (col is < 0 or > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(col), "Must be between 0 and 9");
            }

            if (row is < 0 or > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(row), "Must be between 0 and 9");
            }
            #endregion

            index = GetIndex(col, row);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardIndex"/> type.
        /// </summary>
        /// <param name="location">Location string (e.g. A1, B5, J10) consisting of column (A..J) and row (1..10)</param>
        /// <exception cref="ArgumentOutOfRangeException">Given location has invalid format or index is out of range</exception>
        public BoardIndex(string location)
        {
            // Note range expression. Read more at
            // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-indexes

            // Note out var. Read more at
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-parameter-modifier#calling-a-method-with-an-out-argument

            if (location.Length is >= 2 and <= 3 && location[0] is >= 'A' and <= 'J' && int.TryParse(location[1..], out var row) && row is >= 1 and <= 10)
            {
                index = GetIndex(location[0] - 'A', row - 1);
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(location), "Has to be in the format <column><row> where column is A..J and row is 1..10");
        }
        #endregion

        #region Type conversion and deconstruction
        /// <summary>
        /// Converts a <see cref="BoardIndex"/> into a location string (e.g. A1, B5, J10) consisting of column (A..J) and row (1..10)
        /// </summary>
        /// <param name="value">Value to convert</param>
        public static implicit operator string(BoardIndex value) => $"{(char)('A' + value.Column)}{value.Row + 1}";

        /// <summary>
        /// Converts a <see cref="BoardIndex"/> into a zero-based board index
        /// </summary>
        /// <param name="value">Value to convert</param>
        public static implicit operator int(BoardIndex value) => value.index;

        // Note readonly modifier on method. Read more at
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/readonly-instance-members

        /// <summary>
        /// Deconstructs into column and row indexes
        /// </summary>
        /// <param name="col">Receives zero-based column index</param>
        /// <param name="row">Receives zero-based row index</param>
        public readonly void Deconstruct(out int col, out int row) => (col, row) = (Column, Row);
        #endregion

        #region IEquatable and object
        public readonly override bool Equals(object? obj) => obj is BoardIndex index && Equals(index);

        public readonly bool Equals(BoardIndex other) => index == other.index;

        public readonly override int GetHashCode() => index.GetHashCode();

        public static bool operator ==(BoardIndex left, BoardIndex right) => left.Equals(right);

        public static bool operator !=(BoardIndex left, BoardIndex right) => !(left == right);
        #endregion

        #region Navigation functions
        public readonly BoardIndex NextColumn() => Column < 9 ? new BoardIndex(index + 1) : throw new InvalidOperationException("Already on last column");

        public readonly BoardIndex NextRow() => Row < 9 ? new BoardIndex(index + 10) : throw new InvalidOperationException("Already on last row");
        #endregion

        #region Properties
        public readonly int Column => index % 10;

        public readonly int Row => index / 10;
        #endregion
    }
}
