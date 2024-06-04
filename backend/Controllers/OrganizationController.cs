using Diplomski.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> registerOrganization([FromQuery] string name, [FromQuery] string? email = "")
        {
            return Ok();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> listUsersOrganization()
        {
            return Ok();
        }

        [HttpPost("join")]
        [Authorize]
        public async Task<IActionResult> joinOrganization([FromQuery] string code)
        {
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> leaveOrganization([FromQuery] string id)
        {
            return Ok();
        }
    }
}
