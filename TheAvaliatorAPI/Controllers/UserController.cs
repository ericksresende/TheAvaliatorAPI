using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TheAvaliatorAPI.Model;

namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _httpClient;

        public UserController(ILogger<UserController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                string apiUrl = "https://www.thehuxley.com/api/v1/user";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (Request.Headers.TryGetValue("Authorization", out var header))
                {
                    var authHeaderValue = header.ToString();
                    var token = authHeaderValue.Replace("Bearer ", "");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(responseBody);

                    return Ok(user);
                }
                else
                {
                    return BadRequest("Failed to retrieve user");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving user");
            }
        }
        [HttpGet("{codigoTarefa}/users")]
        public async Task<IActionResult> GetUsersByQuiz(string codigoTarefa)
        {
            try
            {
                string apiUrl = $"https://www.thehuxley.com/api/v1/quizzes/{codigoTarefa}/users?max=100&offset=0";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (Request.Headers.TryGetValue("Authorization", out var header))
                {
                    var authHeaderValue = header.ToString();
                    var token = authHeaderValue.Replace("Bearer ", "");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var classes = JsonConvert.DeserializeObject<List<Problem>>(responseBody);

                    return Ok(classes);
                }
                else
                {
                    return BadRequest("Failed to retrieve classes");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("obtercodigo")]
        public async Task<IActionResult> GetUsersByQuiz()
        {
            try
            {
                string apiUrl = $"https://www.thehuxley.com/api/v1/user";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (Request.Headers.TryGetValue("Authorization", out var header))
                {
                    var authHeaderValue = header.ToString();
                    var token = authHeaderValue.Replace("Bearer ", "");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    System.Console.WriteLine(responseBody);
                    //var classes = JsonConvert.DeserializeObject<List<Problem>>(responseBody);
                    System.Console.WriteLine(responseBody);
                    //return Ok(responseBody);
                    return Content(responseBody, "application/json");
                }
                else
                {
                    return BadRequest("Failed to retrieve classes");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
