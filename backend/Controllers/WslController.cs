using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WslController : Controller
    {
        [HttpGet("list")]
        public IActionResult Index()
        {
            var processInfo = new ProcessStartInfo("wsl", "--list");

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
                    return Ok(output);
                }
                return BadRequest(exitCode);
            }
        }
        [HttpPost("upload")]
        public IActionResult upload(IFormFile file)
        {
            long size = file.Length;
            var filePath = "./Resource/wsl/" + file.FileName;
            try
            {
                using(var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
            return Ok(file.FileName);
        }

        [HttpPost("import")]
        public IActionResult import(string name, string file)
        {
            Directory.CreateDirectory($"./Resource/distro/{name}");

            var processInfo = new ProcessStartInfo("wsl", $"--import {name} ./Resource/distro/{name} ./Resource/wsl/{file}");

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
                    return Ok(output);
                }
                Console.WriteLine(output);
                return BadRequest(exitCode);
            }
        }
        [HttpDelete("unregister")]
        public IActionResult delete(string name) {
            var processInfo = new ProcessStartInfo("wsl", $"--unregister {name}");

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
                    Directory.Delete($"./Resource/distro/{name}");
                    return Ok(output);
                }
                Console.WriteLine(output);
                return BadRequest(exitCode);
            }
        }



    }
}
