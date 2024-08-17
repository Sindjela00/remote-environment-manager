using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {

        private IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        
        [Authorize]
        [HttpGet("sessions")]
        public IActionResult get_session()
        {
            string user = Globals.get_user(Request);
            List<string> session_names = new List<string>();
            foreach (Session session in Globals.sessions.FindAll(x => x.belong(user)))
            {
                session_names.Add(session.get_id());
            }
            return Ok(session_names);
        }
        [Authorize]
        [HttpPost("session")]
        public IActionResult create_session(string name)
        {
            string user = Globals.get_user(Request);
            if (Globals.sessions.FindAll(x => x.get_id() == name).Count() != 0)
            {
                return BadRequest("Name already exists");
            }
            Globals.sessions.Add(new Session(name,  user));
            return Ok();
        }

        [HttpPost("register")]
        public IActionResult register(string email, string password)
        {
            return Ok(DB.register(email, password));
        }
        [HttpPost("login")]
        public IActionResult login(string email, string password)
        {
            int id = DB.login(email, password);
            if (id == -1)
            {
                return BadRequest("");
            }
            var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email)};
            JwtSecurityToken? jwtToken = new JwtSecurityToken(_config["Jwt:Issuer"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256Signature)
            );
            return Ok(new JwtSecurityTokenHandler().WriteToken(jwtToken));
        }
    }

}