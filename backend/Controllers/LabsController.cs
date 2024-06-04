using Diplomski.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabsController : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLab([FromQuery] string lab)
        {
            return Ok();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateLab([FromQuery] string lab)
        {
            return Ok();
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateLab([FromBody] LabsDTO lab)
        {
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteLab([FromQuery] string lab)
        {
            return Ok();
        }
        [HttpPost("execute")]
        [Authorize]
        public async Task<IActionResult> ExecuteDeploy([FromQuery] string lab, string script)
        {
            return Ok();
        }
    }
}
