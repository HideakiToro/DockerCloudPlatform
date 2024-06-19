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
            while (!UserConnector.isOk())
            {
                UserConnector.connect();
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                string name = data?.GetProperty("name").GetString();
                
                DockerController.deleteAllForUser(name);

                UserConnector.SendCommand($"DELETE FROM Users WHERE name = \"{name}\"");

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
            while (!UserConnector.isOk())
            {
                UserConnector.connect();
            }
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var data = JsonSerializer.Deserialize<dynamic>(requestBody);
                string name = data?.GetProperty("name").GetString();
                string password = data?.GetProperty("password").GetString();

                UserConnector.SendCommand($"INSERT INTO Users (name,password) VALUES (\"{name}\",\"{password}\")");

                return Ok(new { message = "Accepted!" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { status = "error", error = e.Message.ToString() });
            }
        }
    }
}