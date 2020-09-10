namespace NBattleshipCodingContest.Manager
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        [HttpPost]
        public Task<IActionResult> Start()
        {
            throw new NotImplementedException();
        }
    }
}
