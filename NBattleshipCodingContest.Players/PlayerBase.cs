namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;
    using System.Threading.Tasks;

    public delegate Task<SquareContent> Shoot(string location);

    public abstract class PlayerBase
    {
        public abstract void GetShot(IReadOnlyBoard board, Shoot shoot);
    }
}
