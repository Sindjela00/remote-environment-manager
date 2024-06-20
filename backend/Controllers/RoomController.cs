using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : Controller
    {
        [HttpGet]
        public IActionResult get_rooms()
        {
            List<Room>? rooms = DB.ListRooms();
            if (rooms == null)
            {
                return BadRequest();
            }
            return Ok(rooms);
        }

        [HttpPost]
        public IActionResult add_room(string room)
        {
            string tmp_room = room.Trim();
            if (tmp_room.Length == 0)
            {
                return BadRequest("Empty name!");
            }
            if (DB.AddRoom(tmp_room))
            {
                return Ok();
            }
            return BadRequest("Error entrying new room!");
        }
        [HttpPut]
        public IActionResult rename_room(uint id, string room)
        {
            string tmp_room = room.Trim();
            if (tmp_room.Length == 0)
            {
                return BadRequest("Empty name!");
            }
            if (DB.RenameRoom(id, tmp_room))
            {
                return Ok();
            }
            return BadRequest("Error renaming room!");
        }
    }
}