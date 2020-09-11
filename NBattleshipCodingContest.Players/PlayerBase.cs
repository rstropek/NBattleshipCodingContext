namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;

    public abstract class PlayerBase
    {
        public abstract string GetShot(IReadOnlyBoard board);
    }
}
