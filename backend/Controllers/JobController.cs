using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : Controller
    {
        [HttpPost("/execute")]
        public IActionResult execute(string file, string container) {

            FileInfo script = new FileInfo(@"./Resource/scripts/"+file);
            string dest_path = "C:/Users/Sindjela/Desktop/docker/materijali/" + script.Name;
            script.CopyTo(dest_path);

             var processInfo = new ProcessStartInfo("docker", $"exec -dt " + container + " /bin/bash -c \"/home/dev/materijali/" + script.Name +"\"");

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
                    System.IO.File.Delete(dest_path);
                    return Ok(output);
                }
                return BadRequest(exitCode);
            }
        }
        [HttpPost("/push")]
        public IActionResult push(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            var filePath = "./Resource/files/" + Path.GetRandomFileName();
            System.IO.Directory.CreateDirectory(filePath);
            filePath += "/";
            foreach(var formFile in files)
            {
                if(formFile.Length > 0)
                {
                    using(var stream = System.IO.File.Create(filePath + formFile.FileName))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }
            DirectoryInfo di = new DirectoryInfo(filePath);
            foreach(var file in di.GetFiles()){
                file.MoveTo("C:/Users/Sindjela/Desktop/docker/materijali/" + file.Name);
            }
            return Ok(new { count = files.Count, size });
        }
        [HttpGet("/pull")]
        public IActionResult Pull()
        {
            string materijali_path = @"C:/Users/Sindjela/Desktop/docker/materijali";
            string zip_path = @"Resource\files\zip\" + Path.GetRandomFileName() + ".zip";
            ZipFile.CreateFromDirectory(materijali_path, zip_path);
            Stream stream = new FileStream(zip_path, FileMode.Open);
            if(stream == null) {
                return NotFound();
            }
            return File(stream, "application/octet-stream", "rezultati.zip");
        }
        [HttpDelete("/clear")]
        public IActionResult clear()
        {
            string materijali_path = @"C:/Users/Sindjela/Desktop/docker/materijali";
            try{
                DirectoryInfo di = new DirectoryInfo(materijali_path);
                foreach( var directory in di.GetDirectories()) {
                    directory.Delete();
                }
                foreach(var file in di.GetFiles()) {
                    file.Delete();
                }
                return Ok();
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
