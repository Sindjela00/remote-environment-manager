using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnsibleController : Controller
    {
        static ProcessHandler handler;

        public AnsibleController() { 
        if (handler == null)
            {
                handler = new ProcessHandler();
            }
        }

        
        [HttpGet("/build")]
        public IActionResult build_image(string predmet, string image, string inventory)
        {
            var files = Directory.GetFiles(@"./Resource/Dockerimages");

            for(int i = 0; i < files.Length; i++)
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
            if(process.Start())
                {
                  var id = ProcessHandler.AddProcess(process);
                  return Ok(id);
                }
            return BadRequest($"Error starting build { process.StandardError.ReadToEnd() }");
        }
        [HttpPost("/start")]
        public IActionResult start_image(string predmet, string image, string inventory)
        {
            var processInfo = new ProcessStartInfo("bash", 
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'name={predmet} dockerimage={image}\' Resource/playbooks/run_container.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if(process.Start())
                {
                  var id = ProcessHandler.AddProcess(process);
                  return Ok(id);
                }
            return BadRequest($"Error starting build { process.StandardError.ReadToEnd() }");
        }
        
        
        [HttpGet("/ansibele_process/{id}")]
        public IActionResult ansible_process_status(ulong id)
        {
            var process = ProcessHandler.GetProcess(id);
            if(process == null) {
                return BadRequest(new { error = "Process not found"});
            }

            Process proc = process.get_process();
            if(!proc.HasExited) {
                return BadRequest(new { error = "Process not finished" });
            }
            if(proc.ExitCode == 0) {
                return Ok(new{status = proc.ExitCode, stdout = proc.StandardOutput.ReadToEnd(), error = proc.StandardError.ReadToEnd() });
            }
            return BadRequest(new { status = proc.ExitCode, stdout = proc.StandardOutput.ReadToEnd(), error = proc.StandardError.ReadToEnd() });
        }

        [HttpGet("/push_files")]
        public IActionResult push_files(string directory, string inventory)
        {
            var processInfo = new ProcessStartInfo("bash", 
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'source={directory}\' Resource/playbooks/push_files.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if(process.Start())
                {
                  var id = ProcessHandler.AddProcess(process);
                  return Ok(id);
                }
            return BadRequest($"Error starting build { process.StandardError.ReadToEnd() }");
        }

        [HttpGet("/pull_files")]
        public IActionResult pull_files(string inventory)
        {
            var processInfo = new ProcessStartInfo("bash", 
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} Resource/playbooks/pull_files.yml\"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if(process.Start())
                {
                  var id = ProcessHandler.AddProcess(process);
                  return Ok(id);
                }
            return BadRequest($"Error starting build { process.StandardError.ReadToEnd() }");
        }
    }
}
