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
    internal class TournamentsController : ControllerBase
    {
        private readonly IEnumerable<PlayerBase> players;
        private readonly BattleHostConnection battleHostConnection;

        public TournamentsController(IEnumerable<PlayerBase> players, BattleHostConnection battleHostConnection)
        {
            this.players = players;
            this.battleHostConnection = battleHostConnection;
        }

        [HttpPost]
        public Task<IActionResult> Start()
        {
            if (players.Count() < 2)
            {
                return Task.FromResult(BadRequest(new ProblemDetails
                {
                    Type = "Configuration error",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Too few players",
                    Detail = "There have to be at least two players in order to start a tournament"
                }) as IActionResult);
            }

            if (!battleHostConnection.IsHostConnected)
            {
                return Task.FromResult(BadRequest(new ProblemDetails
                {
                    Type = "Battle Host Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "No battle host connected to manager",
                    Detail = "There is not battle host connected to the manager. Did you forget to start it?"
                }) as IActionResult);
            }

            var player1 = players.First();
            var player2 = players.Skip(1).First();

            var rbf = new RandomBoardFiller();
            var boardPlayer1 = new BattleshipBoard();
            rbf.Fill(new[] { 2, 3, 3, 4, 5 }, boardPlayer1);
            var boardPlayer2 = new BattleshipBoard();
            rbf.Fill(new[] { 2, 3, 3, 4, 5 }, boardPlayer2);

            while (!boardPlayer1.HasLost && !boardPlayer2.HasLost)
            {
                //var shot1 = player1!.GetShot(boardPlayer2);
                //boardPlayer2.ShootAt(shot1);
                //var shot2 = player2!.GetShot(boardPlayer1);
                //boardPlayer1.ShootAt(shot2);
            }

            return Task.FromResult(Ok() as IActionResult);
        }
    }
}
