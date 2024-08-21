using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Ocsp;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Security.Claims;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActionsController : Controller
    {
        [Authorize]
        [HttpPost("/build_image")]
        public IActionResult build_image(string session, string predmet, string image, string? inventory, List<int>? machines)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = Globals.choose_inventory( ses!, inventory, machines);
            if (Ansible.build_image_func(ses, predmet, image, inv)) {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("/start_container")]
        public IActionResult start_image(string session, string predmet, string image, string? inventory, List<int>? machines)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = Globals.choose_inventory( ses!, inventory, machines);
             if (Ansible.start_image_func(ses, predmet, image, inv)) {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("/stop_container")]
        public IActionResult stop_container(string session, string predmet, string? inventory, List<int>? machines)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = Globals.choose_inventory( ses!, inventory, machines);
             if (Ansible.stop_container_func(ses, predmet, inv)) {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("/upload_files")]
        public IActionResult upload_files(string session, List<IFormFile> files)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            long size = files.Sum(f => f.Length);
            string foldername = Path.GetRandomFileName();
            var filePath = "./Resource/files/" + foldername;
            Directory.CreateDirectory(filePath);
            filePath += "/";
            foreach(var formFile in files)
            {
                if(formFile.Length > 0)
                {
                    using(var stream = System.IO.File.Create( filePath + formFile.FileName))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }
            ses.set_directory(foldername);
            return Ok(new { count = files.Count, size, foldername});
        }
        
        [Authorize]
        [HttpPost("/push_files")]
        public IActionResult push_files(string session, string? directory, string? inventory, List<int>? machines)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = Globals.choose_inventory( ses!, inventory, machines);
            if (directory == null) {
                directory = ses.get_directory();
            }
            if (Ansible.push_files_func(ses, directory,inv)) {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("/pull_files")]
        public IActionResult pull_files(string session, string? inventory, List<int>? machines)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = Globals.choose_inventory( ses!, inventory, machines);
            if (Ansible.pull_files_func(ses, inv)) {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("/download_files")]
        public IActionResult Pull(string session, string? inventory, List<int>? machines)
        {
            string user = Globals.get_user(Request);
            Session? ses = Globals.sessions.Find(x => x.get_id() == session && x.belong(user));
            if (ses == null)
            {
                return BadRequest("You dont have permission for that session!");
            }
            string inv = Globals.choose_inventory( ses!, inventory, machines);
            
            string materijali_path = @"Resource\files\" +ses.get_pull_directory();
            string zip_path = @"Resource\files\zip\" + Path.GetRandomFileName() + ".zip";
            ZipFile.CreateFromDirectory(materijali_path, zip_path);
            Stream stream = new FileStream(zip_path, FileMode.Open);
            if(stream == null) {
                return NotFound();
            }
            return File(stream, "application/octet-stream", "rezultati.zip");
        }

        [Authorize]
        [HttpGet("/process_status")]
        public IActionResult process_status(string session)
        {
            string user = Globals.get_user(Request);
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
                Console.WriteLine(stdout);
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
    }
}
