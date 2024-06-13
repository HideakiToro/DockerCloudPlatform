using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DockerWebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [HttpPost(Name = "login")]
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

                var response = UserConnector.SendCommand($"SELECT * FROM Users WHERE name = \"{name}\" AND password = \"{password}\"");
                bool ok = false;
                Console.WriteLine("Hello world");
                if (response != null)
                {
                    // response.Read();
                    if (response.FieldCount > 0)
                    {
                         response.Read();
                        var res = response.GetString(0);
                        Console.WriteLine("result" + res);
                        ok = res != null ? res != "" : false;
                        Console.WriteLine("hallo");
                    }
                }

                return Ok(new { ok = ok });
            }
            catch (Exception e)
            {
                return BadRequest(new { status = "error", error = "Name already in use" });
            }
        }
    }
}