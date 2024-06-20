using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

namespace DockerWebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DockerController : ControllerBase
    {
        #region Setup
        private readonly ILogger<DockerController> _logger;

        public DockerController(ILogger<DockerController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region GET
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

                string[] ids = MySQLManager.SendCommand($"SELECT Containers.id, Containers.port FROM Containers JOIN Users ON Containers.userID = Users.id WHERE Containers.name = '{name}' AND Users.name = '{user}'");
                if(ids.Length <= 0)
                {
                    return Ok(new { status = "Exited", logs = new string[1] { "Setup failed!" }, port = -1 });
                }
                string id = ids[0].Split(',')[0];
                try {
                    string[] statusStringArr = executeCommand("ps --format \"{{.Status}}\" --filter \"id=" + id + "\"");
                    string status = "Exited";
                    if (statusStringArr.Length > 0)
                    {
                        status = statusStringArr[0].Contains("(Paused)") ? "Paused" : "Up";
                    }
                    string[] logs = executeCommand("logs " + id);

                    int port = int.Parse(ids[0].Split(',')[1]);

                    return Ok(new { status, logs, port });
                } catch {
                    return Ok(new { status = "Exited", logs = new string[1] {"Setup failed!"}, port = -1 });
                }
            }
            else
            {
                Console.WriteLine("Overview Request");
                string[] userContainers = MySQLManager.SendCommand($"SELECT Containers.id, Containers.name FROM Containers JOIN Users ON Containers.userID = Users.id WHERE Users.name = '{user}'");
                if (userContainers.Length > 0) {
                    List<string> ids = new List<string>();
                    foreach (string id in userContainers)
                    {
                        ids.Add(id.Split(',')[0]);
                    }
                    Dictionary<string, string> idsToNames = new Dictionary<string, string>();
                    foreach (string line in userContainers)
                    {
                        idsToNames.Add(line.Split(',')[0], line.Split(',')[1]);
                    }

                    string[] res = executeCommand("ps -a --format \"{{.ID}} {{.Status}}\"");
                    List<object> containers = new List<object>();
                    foreach (string s in res)
                    {
                        string[] parts = s.Split(' ');
                        string containerID = parts[0];
                        string containerStatus = parts[1];

                        if (ids.Contains(containerID))
                        {
                            string containerName = idsToNames[containerID];
                            var container = new { name = containerName, status = containerStatus };
                            containers.Add(container);
                        }
                    }
                    return containers;
                }
                return new List<object>();
            }
        }
        #endregion

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

        #region POST
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

                string[] names = MySQLManager.SendCommand($"SELECT * FROM Containers JOIN Users ON Containers.userID = Users.id WHERE Containers.name = '{name}' AND Users.name = '{user}'");
                if (names.Length > 0)
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

                int portToUse = int.Parse(MySQLManager.SendCommand("SELECT port FROM Ports")[0].Split(',')[0]);
                MySQLManager.SendCommand($"DELETE FROM Ports WHERE port = {portToUse}");

                string startCommand = $"run -p {portToUse}:{port}";
                startCommand += commands.Length > 0 ? $" -e {commands}" : "";
                startCommand += $" -d {image}";

                string[] lines = executeCommand(startCommand);
                string id = "";
                foreach (string line in lines)
                {
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

                string[] users = MySQLManager.SendCommand($"SELECT id FROM Users WHERE name = \"{user}\"");
                string userID = users[0].Split(',')[0];

                MySQLManager.SendCommand($"INSERT INTO Containers (id, name, userID, port) VALUES ('{id}', '{name}', {userID}, {portToUse})");

                return Ok(new { status = "started", image = image, name = name, port = portToUse });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", error = ex.Message });
            }
        }
        #endregion

        #region DELETE
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

                string name = data?.GetProperty("name").GetString();

                string[] userContainers = MySQLManager.SendCommand($"SELECT Containers.id FROM Containers JOIN Users ON Containers.userID = Users.id WHERE Containers.name = '{name}' AND Users.name = '{user}'");

                if (userContainers.Length <= 0)
                {
                    return Ok(new { status = "deleted" });
                }
                string id = userContainers[0].Split(',')[0];
                DeleteContainer(id);

                return Ok(new { status = "deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }

        public static void DeleteContainer(string id)
        {
            //Stop Image
            executeCommand($"stop {id}");

            //Remove Image
            executeCommand($"rm {id}");

            int port = int.Parse(MySQLManager.SendCommand($"SELECT port FROM Containers WHERE id = '{id}'")[0].Split(',')[0]);
            MySQLManager.SendCommand($"INSERT INTO Ports (port) VALUES ({port})");
            MySQLManager.SendCommand($"DELETE FROM Containers WHERE id = '{id}'");
        }

        public static void deleteAllForUser(string user){
            string[] ids = MySQLManager.SendCommand($"SELECT Containers.id FROM Containers JOIN Users ON Containers.userID = Users.id WHERE Users.name = '{user}'");
            foreach (string id in ids)
            {
                DeleteContainer(id.Split(',')[0]);
            }
        }
        #endregion
    }
}
