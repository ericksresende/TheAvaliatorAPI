using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TheAvaliatorAPI.Model;


namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly HttpClient _httpClient;

        public LoginController(ILogger<LoginController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient=httpClient;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                string apiUrl = "https://www.thehuxley.com/api/login";

                var jsonContent = new StringContent(JsonConvert.SerializeObject(request),
                                                    Encoding.UTF8,
                                                    "application/json");

                var response = await _httpClient.PostAsync(apiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

                    return Ok(new { data?.Access_token });
                }
                else
                {
                    return BadRequest("Failed to authenticate user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, "An error occurred during login");
            }
        }
    }
}
