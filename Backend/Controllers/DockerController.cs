using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DockerWebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DockerController : ControllerBase
    {
        private readonly ILogger<DockerController> _logger;

        public DockerController(ILogger<DockerController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetDocker")]
        public IEnumerable<string> Get()
        {
            List<string> result = new List<string>();

            var processInfo = new ProcessStartInfo("docker", $"ps -a");

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;

            int exitCode;
            var process = new Process();
            process.StartInfo = processInfo;

            process.Start();
            Console.WriteLine("process started");
            process.WaitForExit(1200000);
            while (!process.StandardOutput.EndOfStream)
            {
                result.Add(process.StandardOutput.ReadLine());
            }
            Console.WriteLine("process ended");
            if (!process.HasExited)
            {
                process.Kill();
            }

            process.Close();
            Console.WriteLine("Output is as follows:");
            foreach(string s in result)
            {
                Console.WriteLine(s);
            }
            return result.ToArray();
        }

        [HttpPost(Name = "PostDocker")]
        public string Post(string input)
        {
            switch (input)
            {
                case "mysql":
                    var processInfo = new ProcessStartInfo("docker", $"run --name mysql-docker -p 3306:3306 -e MYSQL_ROOT_PASSWORD=password -d mysql");

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardOutput = true;

                    var process = new Process();
                    process.StartInfo = processInfo;

                    process.Start();
                    Console.WriteLine("process started");
                    process.WaitForExit(1200000);
                    Console.WriteLine("process ended");
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Close();
                    return "started mysql container";
                default:
                    return $"unknown image {input}!";
            }
        }
    }
}
