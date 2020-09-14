namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;

    public class Sequential : PlayerBase
    {
        private int nextShot;

        public override void GetShot(IReadOnlyBoard _, Shoot shoot)
        {
            if (nextShot >= 100)
            {
                throw new InvalidOperationException("Next shot index exceeded maximum, invalid program state");
            }

            var result =  $"{(char)('A' + nextShot % 10)}{nextShot / 10 + 1}";
            shoot(result);
            nextShot++;
        }
    }
}
