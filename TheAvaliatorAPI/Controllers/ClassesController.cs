using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TheAvaliatorAPI.Model.ClassesModel;
using TheAvaliatorAPI.Model.Quiz;

namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ClassesController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _httpClient;

        public ClassesController(ILogger<UserController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            try
            {
                string apiUrl = "https://www.thehuxley.com/api/v1/user/groups?max=10&offset=0&order=desc&sort=lastUpdated";

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
                    var classes = JsonConvert.DeserializeObject<List<Group>>(responseBody);

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

        [HttpGet("quiz")]
        public async Task<IActionResult> GetAllQuiz()
        {
            try
            {
                string apiUrl = "https://www.thehuxley.com/api/v1/user/quizzes?filter=OWN&offset=0&order=desc&sort=endDate";

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
                    var classes = JsonConvert.DeserializeObject<List<Quiz>>(responseBody);

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

        [HttpGet("quiz/{codigoTurma}")]
        public async Task<IActionResult> GetAllQuizByClass(string codigoTurma)
        {
            try
            {
                string apiUrl = $"https://www.thehuxley.com/api/v1/groups/{codigoTurma}/quizzes?max=30&offset=0&order=desc&sort=startDate";

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
                    var classes = JsonConvert.DeserializeObject<List<Quiz>>(responseBody);

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
    }
}
