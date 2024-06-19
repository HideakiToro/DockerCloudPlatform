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
            if(user == "")
            {
                return new Dictionary<string, int>();
            }
            if (name != null)
            {
                Console.WriteLine("Status Request");
                if (!nameToID.ContainsKey(user) || !nameToID[user].ContainsKey(name)) 
                {
                    return Ok(new { status = "Exited", logs = new string[1] { "Setup failed!" }, port = -1 });
                }
                string id = nameToID[user][name];
                try {
                    string[] statusStringArr = executeCommand("ps --format \"{{.Status}}\" --filter \"id=" + id + "\"");
                    string status = "Exited";
                    if (statusStringArr.Length > 0)
                    {
                        status = statusStringArr[0].Contains("(Paused)") ? "Paused" : "Up";
                    }
                    string[] logs = executeCommand("logs " + id);
                    int port = containers[id];

                    return Ok(new { status, logs, port });
                } catch {
                    return Ok(new { status = "Exited", logs = new string[1] {"Setup failed!"}, port = -1 });
                }
            }
            else
            {
                Console.WriteLine("Overview Request");
                if (dockerToUser.ContainsKey(user)) {
                    string[] res = executeCommand("ps -a --format \"{{.ID}} {{.Status}}\"");
                    List<object> containers = new List<object>();
                    foreach (string s in res)
                    {
                        string[] parts = s.Split(' ');
                        string containerID = parts[0];
                        string containerStatus = parts[1];

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
            process.WaitForExit(1200000);
            while (!process.StandardOutput.EndOfStream)
            {
                result.Add(process.StandardOutput.ReadLine());
            }
            if (!process.HasExited)
            {
                process.Kill();
            }

            process.Close();
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
            Console.WriteLine("Start Request");
            string user = Request.Cookies["username"] ?? "";
            if (user == "")
            {
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
                process.WaitForExit(1200000);
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
                if (!dockerToUser.ContainsKey(user))
                {
                    dockerToUser.Add(user, new List<string>());
                }
                dockerToUser[user].Add(id);

                if (!nameToID.ContainsKey(user))
                {
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
            Console.WriteLine("Delete Request");
            string user = Request.Cookies["username"] ?? "";
            if (user == "")
            {
                return NotFound(new { status = "User not Found" }); ;
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                //return data;

                string name = data?.GetProperty("name").GetString();

                if (!nameToID.ContainsKey(user) || !nameToID[user].ContainsKey(name))
                {
                    return Ok(new { status = "deleted" });
                }
                string id = nameToID[user][name];

                if (containers.ContainsKey(id))
                {
                    nameToID[user].Remove(name);
                    //Stop Image
                    var processInfo = new ProcessStartInfo("docker", $"stop {id}");

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardOutput = true;

                    var process = new Process();
                    process.StartInfo = processInfo;

                    process.Start();
                    process.WaitForExit(1200000);
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Close();

                    //Remove Image
                    processInfo = new ProcessStartInfo("docker", $"rm {id}");

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardOutput = true;

                    process = new Process();
                    process.StartInfo = processInfo;

                    process.Start();
                    process.WaitForExit(1200000);
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Close();

                    reopenedPorts.Add(containers[id]);
                    containers.Remove(id);

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
