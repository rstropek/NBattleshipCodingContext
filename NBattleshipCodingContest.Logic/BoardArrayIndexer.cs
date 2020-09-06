namespace NBattleshipCodingContest.Logic
{
    using System;
    using System.Diagnostics;

    // Note: *Internal* to make class visible to test project

    /// <summary>
    /// Helper class useful when implementing the battleship board as a continuous array.
    /// </summary>
    /// <remarks>
    /// The implementation assumes the classic battleship game with a side length of 10.
    /// </remarks>
    internal static class BoardArrayIndexer
    {
        /// <summary>
        /// Calculates index based on column and row
        /// </summary>
        /// <param name="col">Zero-based column index</param>
        /// <param name="row">Row-based row index</param>
        /// <returns>
        /// Zero-based index calculated from location string.
        /// </returns>
        private static int GetIndexImpl(int col, int row)
        {
            // This is a private method, so we can assume that col and row
            // contain valid values. We check them only in debug builds.

            // Read more about assertation in C#
            // https://docs.microsoft.com/en-us/visualstudio/debugger/assertions-in-managed-code
            Debug.Assert(col is >= 0 and <= 9 || row is >= 0 and <= 9);

            return row * 10 + col;
        }

        /// <summary>
        /// Get index in continous array of board squares based on a location string.
        /// </summary>
        /// <param name="location">Location string (e.g. A1, B5, J10) consisting of column (A..J) and row (1..10)</param>
        /// <example>
        /// var i = GetIndex("A1");
        /// System.Console.WriteLine(i); // Will result in "0"
        /// </example>
        /// <exception cref="ArgumentException">Thrown in case of invalid argument</exception>
        /// <returns>
        /// Zero-based index calculated from location string.
        /// </returns>
        public static int GetIndex(string location)
        {
            if (location.Length is >= 2 and <= 3 && int.TryParse(location[1..], out var row))
            {
                return GetIndex(location[0], row);
            }

            throw new ArgumentException("Has to be in the format <column><row> where column is A..J and row is 1..10", nameof(location));
        }

        /// <summary>
        /// Get index in continous array of board squares based on column and row.
        /// </summary>
        /// <param name="col">Column (A..J)</param>
        /// <param name="row">One-based row (1..10)</param>
        /// <example>
        /// var i = GetIndex('A', 1);
        /// System.Console.WriteLine(i); // Will result in "0"
        /// </example>
        /// <exception cref="ArgumentException">Thrown in case of invalid arguments</exception>
        /// <returns>
        /// Zero-based index calculated from column and row.
        /// </returns>
        public static int GetIndex(char col, int row)
        {
            #region Check input parameter
            // This is a public method, so we have to check that parameters
            // contain valid values.

            // Note new C# 9 relational pattern matching
            // https://devblogs.microsoft.com/dotnet/welcome-to-c-9-0/#relational-patterns

            if (col is < 'A' or > 'J')
            {
                throw new ArgumentException("Must be between A and J", nameof(col));
            }

            if (row is < 1 or > 10)
            {
                throw new ArgumentException("Must be between 1 and 10", nameof(row));
            }
            #endregion

            return GetIndexImpl(col - 'A', row - 1);
        }

        /// <summary>
        /// Get index in continous array of board squares based on column and row.
        /// </summary>
        /// <param name="col">Zero-based column index</param>
        /// <param name="row">Zero-based row index</param>
        /// <example>
        /// var i = GetIndex(1, 1);
        /// System.Console.WriteLine(i); // Will result in "11"
        /// </example>
        /// <exception cref="ArgumentException">Thrown in case of invalid arguments</exception>
        /// <returns>
        /// Zero-based index calculated from column and row.
        /// </returns>
        public static int GetIndex(int col, int row)
        {
            #region Check input parameter
            // This is a public method, so we have to check that parameters
            // contain valid values.

            if (col is < 0 or > 9)
            {
                throw new ArgumentException("Must be between 0 and 9", nameof(col));
            }

            if (row is < 0 or > 9)
            {
                throw new ArgumentException("Must be between 0 and 9", nameof(row));
            }
            #endregion

            return GetIndexImpl(col, row);
        }
    }
}
