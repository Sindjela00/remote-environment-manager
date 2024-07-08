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

        private static string write_inventory(List<Machine> machines){
            string filename = Path.GetRandomFileName();
            string path = @"./Resource/inventory/" + filename;
            StreamWriter sw = new StreamWriter(path);
            int current_room = 0;
            foreach(Machine machine in machines){
                if(current_room != machine.roomId) {
                    current_room = machine.roomId;
                    sw.WriteLine("[room" + current_room + "]");
                }
                if(machine.port != 0){
                    sw.WriteLine($"{machine.hostname}:{machine.port}");
                }
                else {
                    sw.WriteLine($"{machine.hostname}");
                }
            }
            sw.Close();
            return filename;
        }

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

        [HttpGet("machines")]
        public IActionResult list_machines(){
            List<Machine>? machines = DB.ListMachines();
            if(machines == null){
                return BadRequest("Error retriving machines");
            }
            return Ok(machines);
        }

        [HttpGet("machines/{id}")]
        public IActionResult list_machines(int id){
            List<Machine>? machines = DB.ListMachines(id);
            if(machines == null){
                return BadRequest("Error retriving machines");
            }
            return Ok(machines);
        }
        [HttpPost("machines")]
        public IActionResult add_machine(Machine machine){
            if(DB.AddMachines(machine)){
                return Ok();
            }
            return BadRequest("Error adding machine");
        }
        [HttpPut("machines/{id}")]
        public IActionResult edit_machine(int id, Machine machine){
            if(DB.EditMachines(id, machine)){
                return Ok();
            }
            return BadRequest("Error adding machine");
        }
        [HttpPost("inventory")]
        public IActionResult generate_inventory(List<int> ids){
            List<Machine>? machines = DB.ListMachines(ids);
            if(machines == null){
                return BadRequest("Error finding machines");
            }
            if(machines.Count != ids.Count){
                return BadRequest("Not all machines found");
            }
            machines.OrderBy(x=>x.roomId);

            string filename = write_inventory(machines);

            return Ok(filename);
        }
        [HttpPost("room_inventory")]
        public IActionResult generate_room_inventory(List<int> ids){
            List<Machine>? machines = new List<Machine>();
            foreach(int id in ids){
                List<Machine>? mac = DB.ListMachines(id);
                if (mac == null) {
                    continue;
                }   
                machines.AddRange(mac);
            }
            if(machines == null){
                return BadRequest("Error finding machines");
            }
            machines.OrderBy(x=>x.roomId);

            string filename = write_inventory(machines);

            return Ok(filename);
        }
    }
}