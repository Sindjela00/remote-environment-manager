using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;

public static class Globals {
    public static List<Session> sessions = new List<Session>();
    public static string get_user(HttpRequest request)
        {
            var token = request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
    public static string? write_inventory(List<Machine> machines)
        {
            string filename = Path.GetRandomFileName();
            string path = @"./Resource/inventory/" + filename;
            StreamWriter sw = new StreamWriter(path);
            int current_room = 0;

            List<Room>? rooms = DB.ListRooms();
            if (rooms == null || rooms.Count == 0)
            {
                return null;
            }
            foreach (Machine machine in machines)
            {
                if (current_room != machine.roomId)
                {
                    current_room = machine.roomId;
                    Room? room = rooms.Find(x => x.id == current_room);
                    if (room != null)
                    {
                        sw.WriteLine("[" + room.name + "]");
                    }
                }
                if (machine.port != 0)
                {
                    sw.WriteLine(value: $"{machine.hostname} ansible_ssh_host={machine.ipv4} ansible_ssh_port={machine.port} ansible_ssh_pass=machine ansible_ssh_user=machine ansible_sudo_pass=machine");
                    //sw.WriteLine($"{machine.hostname}:{machine.port}");
                }
                else
                {
                    sw.WriteLine(value: $"{machine.hostname} ansible_ssh_host={machine.ipv4} ansible_ssh_pass=machine ansible_ssh_user=machine ansible_sudo_pass=machine");
                    //sw.WriteLine($"{machine.hostname}");
                }
            }
            sw.Close();
            return filename;
        }
        public static string choose_inventory(Session session, string? inventory, List<int>? machines)
        {
            if (machines != null && machines.Count > 0)
            {
                return Globals.write_inventory(DB.ListMachines(machines));
            }

            if (inventory != null)
            {
                return session.get_inventory(inventory);
            }
            return session.get_inventory();
        }
}