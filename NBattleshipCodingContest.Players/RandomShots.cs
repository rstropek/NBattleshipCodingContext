namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;

    public class RandomShots : PlayerBase
    {
        public override string GetShot(IReadOnlyBoard board)
        {
            // Return a random shot between A1 and J10
            var rand = new Random();
            return $"{'A' + rand.Next(1, 11)}{rand.Next(1, 11)}";
        }
    }
}
