namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;

    /// <summary>
    /// Implements a battleship player that randomly shoots at squares
    /// </summary>
    public class RandomShots : PlayerBase
    {
        /// <inheritdoc />
        public override void GetShot(Guid _, string __, IReadOnlyBoard ___, Shoot shoot)
        {
            // Return a random shot between A1 and J10
            var rand = new Random();
            shoot($"{(char)('A' + rand.Next(10))}{rand.Next(1, 11)}");
        }
    }
}
