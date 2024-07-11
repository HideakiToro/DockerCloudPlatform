using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;

namespace DockerWebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        /*
       DELETE /api/Users
       {
           name: ""
       } 
       */

        [HttpDelete(Name = "DeleteUser")]
        public async Task<dynamic> Delete()
        {
            while (!MySQLManager.isOk())
            {
                MySQLManager.connect();
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                string name = data?.GetProperty("name").GetString();
                
                DockerController.deleteAllForUser(name);

                MySQLManager.SendCommand($"DELETE FROM Users WHERE name = \"{name}\"");

                return Ok(new { deleted = true });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { status = "error", error = e.Message.ToString() });
            }
        }

        /*
       POST /api/User
       {
           name: "",
           password: ""
       }
       */

        [HttpPost(Name = "CreateUser")]
        public async Task<dynamic> Post()
        {
            while (!MySQLManager.isOk())
            {
                MySQLManager.connect();
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                string name = data?.GetProperty("name").GetString();
                string password = data?.GetProperty("password").GetString();

            // select * from users where name = "{name}"
            //.send comand prüfen, ob length = "0"
            //wenn nicht null --> return StatusCode(403, new { status = "error", error = e.Message.ToString() });

                MySQLManager.SendCommand($"INSERT INTO Users (name,password) VALUES (\"{name}\",\"{password}\")");

                return Ok(new { message = "Accepted!" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { status = "error", error = e.Message.ToString() });
            }
        }

        [HttpPut(Name = "ChangeUser")]
        public async Task<dynamic> Put()
        {
            string user = Request.Cookies["username"] ?? "";
            if(user == "")
            {
                return new Dictionary<string, int>();
            }
            while (!MySQLManager.isOk())
            {
                MySQLManager.connect();
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                string password = data?.GetProperty("password").GetString();

                Console.WriteLine( $"UPDATE Users SET password = \"{password}\" WHERE name = \"{user}\"");

                MySQLManager.SendCommand($"UPDATE Users SET password = \"{password}\" WHERE name = \"{user}\"");

                return Ok(new { message = "Accepted!" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { status = "error", error = e.Message.ToString() });
            }
        }
    }
}