using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto.Digests;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewAnsibleController : Controller
    {
        static ProcessHandler handler;

        public NewAnsibleController()
        {
            if (handler == null)
            {
                handler = new ProcessHandler();
            }
        }
        
        [Authorize]
        [HttpPost("/build1")]
        public IActionResult build_image(string session, string predmet, string image, string? inventory, List<int>? machines)
        {
            string user = get_user();
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = choose_inventory(user, ses!, inventory, machines);

            return build_image_func(ses, predmet, image, inv);
        }

        
        [HttpPost("/start1")]
        public IActionResult start_image(string session, string predmet, string image, string? inventory, List<int>? machines)
        {
            string user = get_user();
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = choose_inventory(user, ses!, inventory, machines);

            return start_image_func(ses, predmet, image, inv);
        }


        [HttpGet("/ansibele_process")]
        public IActionResult ansible_process_status(string session)
        {
            string user = get_user();
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }

            List<dynamic> output = new List<dynamic>();
            foreach (var dynobj in ses.jobs_result())
            {
                if (dynobj.status == "Process not Finished.")
                {
                    output.Add(dynobj);
                    continue;
                }
                string stdout = dynobj.stdout;
                List<string> filteredList = new List<string>();
                if (stdout.Length > 0)
                {
                    string[] stdoutlist = stdout.Split(separator: '\n');
                    bool flag = false;
                    foreach (string item in stdoutlist)
                    {
                        if (flag && item.Length > 0)
                        {
                            filteredList.Add(item);
                            continue;
                        }
                        if (item.Contains("PLAY RECAP"))
                        {
                            flag = true;
                        }
                    }
                    List<object>? list1 = new List<object>();
                    foreach (string item in filteredList)
                    {
                        string[] split1 = item.Split(separator: ':');
                        string hostname = split1[0].Trim();
                        List<string> statuses = split1[1].Trim().Split("    ").ToList();
                        int ok = int.Parse(statuses[0].Split('=')[1].Trim());
                        int changed = int.Parse(statuses[1].Split('=')[1].Trim());
                        int unreachable = int.Parse(statuses[2].Split('=')[1].Trim());
                        int failed = int.Parse(statuses[3].Split('=')[1].Trim());
                        int skipped = int.Parse(statuses[4].Split('=')[1].Trim());
                        int rescued = int.Parse(statuses[5].Split('=')[1].Trim());
                        int ignored = int.Parse(statuses[6].Split('=')[1].Trim());
                        list1.Add(new
                        {
                            hostname = hostname,
                            ok = ok,
                            changed = changed,
                            unreachable = unreachable,
                            failed = failed,
                            skipped = skipped,
                            rescued = rescued,
                            ignored = ignored
                        });
                    }
                output.Add(new { status = dynobj.status, stdout = list1, error = dynobj.stderr });
                }
            }
            return Ok(output);
        }
        
        [HttpPost("/push_files1")]
        public IActionResult push_files(string session, string directory, string? inventory, List<int>? machines)
        {
            string user = get_user();
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = choose_inventory(user, ses, inventory, machines);
            return push_files_func(ses, directory,inv);
        }

        [HttpPost("/pull_files1")]
        public IActionResult pull_files(string session, string? inventory, List<int>? machines)
        {
            string user = get_user();
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = choose_inventory(user, ses!, inventory, machines);
            return pull_files_func(ses, inv);
        }
        private IActionResult pull_files_func(Session session, string inventory)
        {
            var processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} Resource/playbooks/pull_files.yml\"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return Ok();
            }
            return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
        private IActionResult build_image_func(Session session, string predmet, string image, string inventory)
        {
            var files = Directory.GetFiles(@"./Resource/Dockerimages");

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            string file = files.ToList().Find(files => files.Contains(image));
            if (file == null)
            {
                return BadRequest("Image not found");
            }
            var processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'name={predmet} dockerimage={image}\' Resource/playbooks/build_docker.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return Ok();
            }
            return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
        

        private string choose_inventory(string id, Session session, string? inventory, List<int>? machines)
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

        private bool has_access(string id, string session)
        {
            return Globals.sessions.Find(x => x.get_id() == session && x.owner(id)) != null;
        }

        private string get_user()
        {
            var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
        private IActionResult start_image_func(Session session, string predmet, string image, string inventory)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'name={predmet} dockerimage={image}\' Resource/playbooks/run_container.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process? process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return Ok();
            }
            return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
        private IActionResult push_files_func(Session session, string directory, string inventory)
        {
            var processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'source={directory}\' Resource/playbooks/push_files.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return Ok();
            }
            return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
    }
}
