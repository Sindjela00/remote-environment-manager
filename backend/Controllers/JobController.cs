using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Diplomski.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : Controller
    {
        [HttpGet]
        public ContentResult Index()
        {
            var html = System.IO.File.ReadAllText(@"./Resource/html/jobs.html");
            return base.Content(html, "text/html");
        }

        [HttpPost("/execute")]
        public IActionResult execute(string file) {
            if (!System.IO.File.Exists(file))
            {
                return BadRequest("File {0} doesn't exists");
            }

            return Ok();
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
            Directory.Move(filePath, "C:/Users/Sindjela/Desktop/docker/materijali");
            return Ok(new { count = files.Count, size });
        }
    }
}
