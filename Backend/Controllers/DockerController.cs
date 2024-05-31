using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpGet(Name = "GetContainers")]
        public async Task<dynamic> Get()
        {
            try
            {
                string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                string name = data?.GetProperty("name").GetString();

                string status = executeCommand("ps --format \"{{.Status}}\" --filter \"name=" + name + "\"")[0].Split(' ')[0];
                string[] logs = executeCommand("logs " + name);
                int port = containers[name];

                return Ok(new { status = status, logs = logs, port = port });
            }
            catch
            {
                return executeCommand("ps -a --format \"{{.Names}}\"");
            }
        }

        public static string[] executeCommand(string command)
        {
            List<string> result = new List<string>();

            var processInfo = new ProcessStartInfo("docker", command);

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
            foreach (string s in result)
            {
                Console.WriteLine(s);
            }
            return result.ToArray();
        }

        public static int ports = 6000;
        public static List<int> reopenedPorts = new List<int>();
        public static Dictionary<string, int> containers = new Dictionary<string, int>();

        /*
         * POST /api/Docker
         * {
         *    name: "",
         *    image: "",
         *    port: 0,
         *    command(optional): ""
         * }
         */
        [HttpPost(Name = "StartNewContainer")]
        public async Task<dynamic> Post()
        {
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);

                string name = data?.GetProperty("name").GetString();
                if (containers.ContainsKey(name))
                {
                    return BadRequest(new { status = "error", error = "Name already in use" });
                }
                string image = data?.GetProperty("image").GetString();
                string port = data?.GetProperty("port").GetString();
                string commands = "";
                try
                {
                    commands = data?.GetProperty("commands").GetString();
                }
                catch { }

                int portToUse = 0;
                if (reopenedPorts.Count > 0)
                {
                    portToUse = reopenedPorts[0];
                    reopenedPorts.RemoveAt(0);
                }
                else
                {
                    portToUse = ports;
                    ports++;
                }

                string startCommand = $"run --name {name} -p {portToUse}:{port}";
                startCommand += commands.Length > 0 ? $" -e {commands}" : "";
                startCommand += $" -d {image}";

                var processInfo = new ProcessStartInfo("docker", startCommand);

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
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (line.Contains("docker: Error response from daemon: Conflict. The container name"))
                    {
                        return BadRequest(new { status = "error", error = "Name already in use" });
                    }
                    else if (line.Contains("error during connect:"))
                    {
                        return StatusCode(500, new { status = "error", error = "Docker not running on Server" });
                    }
                }
                process.Close();

                containers.Add(name, portToUse);

                return Ok(new { status = "started", image = image, name = name, port = portToUse });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", error = ex.Message });
            }
        }

        /*
         * DELETE /api/Docker
         * {
         *    name: ""
         * }
         */
        [HttpDelete(Name = "DeleteContainer")]
        public async Task<dynamic> Delete()
        {
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                //return data;

                string name = data?.GetProperty("name").GetString();
                if (containers.ContainsKey(name))
                {
                    //Get ID
                    Console.WriteLine("deleting: " + name);
                    var processInfo = new ProcessStartInfo("docker", $"ps -a -q --filter ancestor={name} --format=\"{{.ID}}\"");

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardOutput = true;

                    var process = new Process();
                    process.StartInfo = processInfo;

                    process.Start();
                    Console.WriteLine("process started");
                    process.WaitForExit(1200000);
                    string id = "";
                    while (!process.StandardOutput.EndOfStream)
                    {
                        id = process.StandardOutput.ReadLine();
                    }
                    Console.WriteLine("process ended");
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Close();

                    //Stop Image
                    Console.WriteLine("Stopping Container...");
                    processInfo = new ProcessStartInfo("docker", $"stop {id}");

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardOutput = true;

                    process = new Process();
                    process.StartInfo = processInfo;

                    process.Start();
                    Console.WriteLine("process started");
                    process.WaitForExit(1200000);
                    while (!process.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(process.StandardOutput.ReadLine());
                    }
                    Console.WriteLine("process ended");
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Close();

                    //Remove Image
                    Console.WriteLine("Removing Container...");
                    processInfo = new ProcessStartInfo("docker", $"rm {id}");

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardOutput = true;

                    process = new Process();
                    process.StartInfo = processInfo;

                    process.Start();
                    Console.WriteLine("process started");
                    process.WaitForExit(1200000);
                    while (!process.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(process.StandardOutput.ReadLine());
                    }
                    Console.WriteLine("process ended");
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Close();

                    reopenedPorts.Add(containers[name]);
                    containers.Remove(name);
                    Console.WriteLine($"Container {name} removed");

                    return Ok(new { status = "deleted" });
                }
                else
                {
                    return NotFound(new { status = "Not Found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }
    }
}
