using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

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

        public static Dictionary<string, List<string>> dockerToUser = new Dictionary<string, List<string>>();
        public static Dictionary<string, Dictionary<string, string>> nameToID = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, string> idToName = new Dictionary<string, string>();

        [HttpGet(Name = "GetContainers")]
        public async Task<dynamic> Get(string? name = null)
        {
            string user = Request.Cookies["username"] ?? "";
            Console.WriteLine($"Request by {user}");
            if(user == "")
            {
                Console.WriteLine("No Usename set");
                return new Dictionary<string, int>();
            }
            Console.WriteLine("Usename set");
            if (name != null)
            {
                Console.WriteLine("Query");
                Console.WriteLine("User known: " + nameToID.ContainsKey(user));
                Console.WriteLine("Name known: " + nameToID[user].ContainsKey(name));
                if (!nameToID.ContainsKey(user) || !nameToID[user].ContainsKey(name)) 
                {
                    return Ok(new { status = "Exited", logs = new string[1] { "Setup failed!" }, port = -1 });
                }
                Console.WriteLine("Getting Container Info");
                string id = nameToID[user][name];
                try {
                    string[] statusStringArr = executeCommand("ps --format \"{{.Status}}\" --filter \"id=" + id + "\"");
                    Console.WriteLine("Got Status");
                    string status = "Exited";
                    if (statusStringArr.Length > 0)
                    {
                        status = statusStringArr[0].Contains("(Paused)") ? "Paused" : "Up";
                    }
                    Console.WriteLine("Converted Status");
                    string[] logs = executeCommand("logs " + id);
                    Console.WriteLine("Got Logs");
                    int port = containers[id];
                    Console.WriteLine("Got Port");

                    return Ok(new { status, logs, port });
                } catch {
                    return Ok(new { status = "Exited", logs = new string[1] {"Setup failed!"}, port = -1 });
                }
            }
            else
            {
                Console.WriteLine("Full");
                if (dockerToUser.ContainsKey(user)) {
                    string[] res = executeCommand("ps -a --format \"{{.ID}} {{.Status}}\"");
                    List<object> containers = new List<object>();
                    foreach (string s in res)
                    {
                        string[] parts = s.Split(' ');
                        string containerID = parts[0];
                        string containerStatus = parts[1];

                        Console.WriteLine(containerID + " : " + dockerToUser[user].Contains(containerID));

                        if (dockerToUser[user].Contains(containerID))
                        {
                            string containerName = idToName[containerID];
                            var container = new { name = containerName, status = containerStatus };
                            containers.Add(container);
                        }
                    }
                    return containers;
                }
                return new List<object>();
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
            string user = Request.Cookies["username"] ?? "";
            Console.WriteLine($"Request by {user}");
            if (user == "")
            {
                Console.WriteLine("No Usename set");
                return BadRequest(new { status = "error", error = "User missing" });
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);

                string name = data?.GetProperty("name").GetString();
                if (nameToID.ContainsKey(user) && nameToID[user].ContainsKey(name))
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

                string startCommand = $"run -p {portToUse}:{port}";
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
                string id = "";
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
                    else
                    {
                        id = line.Substring(0, 12);
                    }
                }
                process.Close();

                containers.Add(id, portToUse);
                Console.WriteLine($"Adding {id} to list of {user}.");
                if (!dockerToUser.ContainsKey(user))
                {
                    Console.WriteLine($"Adding {user} to the dictionary");
                    dockerToUser.Add(user, new List<string>());
                }
                dockerToUser[user].Add(id);

                Console.WriteLine($"Adding {name}:{id} to dictionary of {user}.");
                if (!nameToID.ContainsKey(user))
                {
                    Console.WriteLine($"Adding {user} to the dictionary");
                    nameToID.Add(user, new Dictionary<string, string>());
                }
                nameToID[user].Add(name, id);
                idToName.Add(id, name);

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
