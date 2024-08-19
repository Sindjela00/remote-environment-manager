using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpPost("inventory")]
        public IActionResult generate_inventory(string session, List<int> ids){
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            List<Machine>? machines = DB.ListMachines(ids);
            if(machines == null){
                return BadRequest("Error finding machines");
            }
            if(machines.Count != ids.Count){
                return BadRequest("Not all machines found");
            }
            machines.OrderBy(x=>x.roomId);

            string filename = Globals.write_inventory(machines);
            ses.set_inventory(filename, machines);
            return Ok(filename);
        }
        [Authorize]
        [HttpPut("inventory")]
        public IActionResult addto_inventory(string session, List<int> ids){
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            List<Machine>? machines = DB.ListMachines(ids);
            if(machines == null){
                return BadRequest("Error finding machines");
            }
            if(machines.Count != ids.Count){
                return BadRequest("Not all machines found");
            }
            machines.AddRange(ses.get_machines());
            machines.OrderBy(x=>x.roomId);

            string filename = Globals.write_inventory(machines);
            ses.set_inventory(filename, machines);
            return Ok(filename);
        }
        [Authorize]
        [HttpDelete("inventory")]
        public IActionResult removefrom_inventory(string session, List<int> ids){
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            List<Machine> machines = ses.get_machines();
  
            if(machines.Count != ids.Count){
                return BadRequest("No more machines");
            }
            foreach(int id in ids) {
                machines.RemoveAll(x=>x.id == id);
            }
            machines.OrderBy(x=>x.roomId);

            string filename = Globals.write_inventory(machines);
            ses.set_inventory(filename, machines);
            return Ok(filename);
        }
        [Authorize]
        [HttpPost("room_inventory")]
        public IActionResult generate_room_inventory(string session, List<int> ids){
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
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

            string filename = Globals.write_inventory(machines);
            ses.set_inventory(filename, machines);
            return Ok(filename);
        }
        [Authorize]
        [HttpPut("room_inventory")]
        public IActionResult add_to_room_inventory(string session, List<int> ids){
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
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
            if(machines.Count != ids.Count){
                return BadRequest("Not all machines found");
            }
            machines.AddRange(ses.get_machines());
            machines.OrderBy(x=>x.roomId);

            string filename = Globals.write_inventory(machines);
            ses.set_inventory(filename, machines);
            return Ok(filename);
        }
        [Authorize]
        [HttpDelete("room_inventory")]
        public IActionResult remove_from_room_inventory(string session, List<int> ids){
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            List<Machine> machines = ses.get_machines();
  
            if(machines.Count != ids.Count){
                return BadRequest("No more machines");
            }
            List<Machine>? delete_machines = new List<Machine>();
            foreach(int id in ids){
                List<Machine>? mac = DB.ListMachines(id);
                if (mac == null) {
                    continue;
                }   
                delete_machines.AddRange(mac);
            }
            foreach(Machine machine in delete_machines) {
                machines.RemoveAll(x=>x.id == machine.id);
            }
            machines.OrderBy(x=>x.roomId);

            string filename = Globals.write_inventory(machines);
            ses.set_inventory(filename, machines);
            return Ok(filename);
        }
    }
}