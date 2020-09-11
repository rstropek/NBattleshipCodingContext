namespace NBattleshipCodingContest.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NBattleshipCodingContest.Players;

    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly IEnumerable<PlayerBase> players;

        public TournamentsController(IEnumerable<PlayerBase> players)
        {
            this.players = players;
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


            return Task.FromResult(Ok() as IActionResult);
        }
    }
}
