using Diplomski.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace Diplomski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginInfo)
        {
            return Ok();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerInfo)
        {
            return Ok();
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RegisterDTO updateInfo)
        {
            return Ok();
        }

        [HttpPut("changepwd")]
        [Authorize]

        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeDTO passwordInfo )
        {
            return Ok();
        }
    }
}
