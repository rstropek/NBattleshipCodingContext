namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;

    public class RandomShots : PlayerBase
    {
        public override void GetShot(IReadOnlyBoard _, Shoot shoot)
        {
            // Return a random shot between A1 and J10
            var rand = new Random();
            shoot($"{(char)('A' + rand.Next(10))}{rand.Next(1, 11)}");
        }
    }
}
