namespace NBattleshipCodingContest.Logic
{
    using System;

    /// <summary>
    /// Checks if a given square on the battleship board is water.
    /// </summary>
    /// <param name="col">Zero-based column index</param>
    /// <param name="row">Zero-based row index</param>
    /// <returns>
    /// <c>true</c> if the given square is water, otherwise <c>false</c>.
    /// </returns>
    internal delegate bool IsWater(int col, int row);

    internal static class ShipPlacementChecker
    {
        /// <summary>
        /// Finds out if a ship can be places at given coordinates.
        /// </summary>
        /// <param name="col">Zero-based column index</param>
        /// <param name="row">Zero-based row index</param>
        /// <param name="shipLength">Length of the ship (max. 10)</param>
        /// <param name="direction">Direction of the ship</param>
        /// <param name="isWater">Callback to find out if a given suqare is water before placing the ship</param>
        /// <returns>
        /// <c>true</c> if the ship can be placed here, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Each ship occupies a number of consecutive squares on the battleship board, arranged either horizontally 
        /// or vertically. The ships cannot overlap (i.e., only one ship can occupy any given square on the board).
        /// There has to be at least one square with water between each ship (i.e., ships must not be place
        /// directly adjacent to each other).
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown in case of invalid argument</exception>
        public static bool CanPlaceShip(BoardIndex ix, int shipLength, Direction direction, IsWater isWater)
        {
            #region Check input parameter
            // This is a public method, so we have to check that parameters
            // contain valid values.

            // Check if ship isn't too long
            if (shipLength > 10)
            {
                throw new ArgumentException("Maximum length of ship is 10", nameof(shipLength));
            }

            if (direction > Direction.Vertical)
            {
                throw new ArgumentException("Unknown direction", nameof(direction));
            }
            #endregion

            // Check if ship is placed outside bounds of board
            static bool OutsideBounds(int start, int shipLength) => start + shipLength > 10;
            if (OutsideBounds(direction == Direction.Horizontal ? ix.Column : ix.Row, shipLength))
            {
                return false;
            }

            // Helper methods for index calculations
            // Note static local functions
            static int GetFirst(int index) => index == 0 ? index : index - 1;
            static int GetElementsToCheckAcross(int index) => (index == 0 || index == 10 - 1) ? 2 : 3;
            static int GetElementsToCheckAlongside(int index, int shipLength) =>
                shipLength + ((index == 0 || index + shipLength == 10) ? 1 : 2);

            var numberOfRowsToCheck = direction == Direction.Horizontal
                ? GetElementsToCheckAcross(ix.Row)
                : GetElementsToCheckAlongside(ix.Row, shipLength);
            var numberOfColsToCheck = direction == Direction.Horizontal
                ? GetElementsToCheckAlongside(ix.Column, shipLength)
                : GetElementsToCheckAcross(ix.Column);

            var firstCheckRow = GetFirst(ix.Row);
            var firstCheckCol = GetFirst(ix.Column);

            // Check if ships overlap
            for (var r = firstCheckRow; r < firstCheckRow + numberOfRowsToCheck; r++)
            {
                for (var c = firstCheckCol; c < firstCheckCol + numberOfColsToCheck; c++)
                {
                    if (!isWater(c, r))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
