using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DockerController : Controller
    {
        static ProcessHandler handler;

        public DockerController() { 
        if (handler == null)
            {
                handler = new ProcessHandler();
            }
        }

        [HttpGet("/images")]
        public IActionResult get_images(bool? ready)
        {
            if(ready == false)
            {
                var files = Directory.GetFiles(@"./Resource/Dockerimages");

                for(int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                return Ok(files);
            }
            var processInfo = new ProcessStartInfo("docker", $"images");

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            int exitCode;
            using(var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();
                process.WaitForExit(30000);
                if(!process.HasExited)
                {
                    process.Kill();
                }
                string output = process.StandardOutput.ReadToEnd();
                exitCode = process.ExitCode;
                process.Close();
                if(exitCode == 0)
                {
                    var strings = output.Split('\n').ToList();
                    strings.RemoveAt(0);
                    strings.RemoveAll(string.IsNullOrEmpty);
                    return Ok(strings);
                }
                return BadRequest(exitCode);
            }
        }
        [HttpGet("/image/build")]
        public IActionResult build_image(string image, string tag)
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

            var processInfo = new ProcessStartInfo("docker", 
                ("buildx build " + (String.IsNullOrEmpty(tag) ? "" : $"-t { tag } ") 
                + $" -f ./Resource/Dockerimages/{image} ."));
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if(process.Start())
                {
                  var id = handler.AddProcess(process);
                  return Ok(id);
                }
            return BadRequest($"Error starting build { process.StandardError.ReadToEnd() }");
        }
        [HttpPost("/image/start")]
        public IActionResult start_image(string image, string? command)
        {
            var processInfo = new ProcessStartInfo("docker", $"run -dt -v C:/Users/Sindjela/Desktop/docker:/home/dev -w /home/dev {image}:latest " + (string.IsNullOrEmpty(command) ? "/bin/bash" : command));

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            int exitCode;
            using(var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();
                process.WaitForExit(30000);
                if(!process.HasExited)
                {
                    process.Kill();
                }
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                exitCode = process.ExitCode;
                process.Close();
                if(exitCode != 0)
                {
                    return BadRequest("Cant start container. error=" + error);
                }
           
                return Ok(output);
            }
        }
        
        [HttpGet("/process/{id}")]
        public IActionResult process_status(ulong id)
        {
            var process = handler.GetProcess(id);
            if(!process.process.HasExited)
            {
                return BadRequest(new { error = "Process not finished" });
            }
            if(process.process.ExitCode == 0)
            {
                return Ok(new{status = process.process.ExitCode, stdout = process.process.StandardOutput.ReadToEnd(), error = process.process.StandardError.ReadToEnd() });
            }
            return BadRequest(new { status = process.process.ExitCode, stdout = process.process.StandardOutput.ReadToEnd(), error = process.process.StandardError.ReadToEnd() });
        }
    }
}
