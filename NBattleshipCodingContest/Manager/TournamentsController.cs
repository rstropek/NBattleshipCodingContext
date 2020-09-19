namespace NBattleshipCodingContest.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NBattleshipCodingContest.Logic;
    using NBattleshipCodingContest.Players;

    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly IEnumerable<PlayerBase> players;
        private readonly BattleHostConnection battleHostConnection;

        public TournamentsController(IEnumerable<PlayerBase> players, BattleHostConnection battleHostConnection)
        {
            this.players = players;
            this.battleHostConnection = battleHostConnection;
        }

        [HttpPost]
        public async Task<IActionResult> Start()
        {
            if (players.Count() < 2)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "Configuration error",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Too few players",
                    Detail = "There have to be at least two players in order to start a tournament"
                });
            }

            if (!battleHostConnection.IsHostConnected)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "Battle Host Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "No battle host connected to manager",
                    Detail = "There is not battle host connected to the manager. Did you forget to start it?"
                });
            }

            var player1 = players.First();
            var player2 = players.Skip(1).First();

            var rbf = new RandomBoardFiller();
            var boardPlayer1 = new BattleshipBoard();
            rbf.Fill(new[] { 2, 3, 3, 4, 5 }, boardPlayer1);
            var boardPlayer2 = new BattleshipBoard();
            rbf.Fill(new[] { 2, 3, 3, 4, 5 }, boardPlayer2);

            var shotsPlayer1 = new BoardContent();
            shotsPlayer1.Clear(Logic.SquareContent.Unknown);
            var shotsPlayer2 = new BoardContent();
            shotsPlayer2.Clear(Logic.SquareContent.Unknown);

            var gameId = Guid.NewGuid();

            while (!shotsPlayer1.HasLost && !shotsPlayer2.HasLost)
            {
                await battleHostConnection.Shoot(gameId, 0, 1, shotsPlayer1, boardPlayer2);
                await battleHostConnection.Shoot(gameId, 1, 0, shotsPlayer2, boardPlayer1);

                Console.WriteLine(shotsPlayer1);
                Console.WriteLine(boardPlayer2);

                Console.WriteLine(shotsPlayer2);
                Console.WriteLine(boardPlayer1);
            }

            return Ok();
        }
    }
}
