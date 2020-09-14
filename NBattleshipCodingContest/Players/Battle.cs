namespace NBattleshipCodingContest.Players
{
    using NBattleshipCodingContest.Logic;
    using System;

    internal class Battle
    {
        private readonly PlayerBase[] players = new PlayerBase[2];
        private readonly BattleshipBoard[] boards = new BattleshipBoard[2];

        public Battle(int player1, int player2)
        {
            if (player1 < 0 || player1 >= PlayerList.Players.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(player1));

            }

            if (player2 < 0 || player2 >= PlayerList.Players.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(player2));
            }

            players[0] = PlayerList.Players[player1];
            players[1] = PlayerList.Players[player2];

            for (var i = 0; i < 2; i++)
            { 
                boards[i] = new BattleshipBoard();
                boards[i].Initialize();
            }
        }

        public void Shoot(int player, Shoot shoot)
        {
            if (player is not 0 or 1)
            {
                throw new ArgumentOutOfRangeException(nameof(player), "Player index must be 0 or 1");
            }


        }
    }
}
