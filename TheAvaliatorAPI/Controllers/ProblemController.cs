using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TheAvaliatorAPI.Model;
using TheAvaliatorAPI.Model.Quiz;

namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProblemController : ControllerBase
    {
        private readonly ILogger<ProblemController> _logger;
        private readonly HttpClient _httpClient;

        public ProblemController(ILogger<ProblemController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("problems")]
        public async Task<IActionResult> GetProblems(string? problemName)
        {
            try
            {
                problemName ??= "";

                var queryString = "max=5";
                //if (string.IsNullOrEmpty(problemName))
                //{
                //    return BadRequest("Query string is required");
                //}
                var problemNameWithoutEspace = Regex.Replace(problemName, @"\s", "+");
                problemNameWithoutEspace = Regex.Replace(problemNameWithoutEspace, @"\++", "+");

                string apiUrl = "https://www.thehuxley.com/api/v1/problems";
                string requestUrl = "";
                requestUrl = string.IsNullOrEmpty(problemName) ? $"{apiUrl}?{queryString}" : $"{apiUrl}?{queryString}&q={problemNameWithoutEspace}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
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
                    var submissions = JsonConvert.DeserializeObject<List<Problem>>(responseBody);

                    return Ok(submissions);
                }

                return BadRequest("Failed to retrieve submissions");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving submissions");
            }
        }
        
        
        [HttpGet("problems/quiz/{codigoTarefa}")]
        public async Task<IActionResult> GetProblemsByQuiz(string codigoTarefa)
        {
            try
            {
                string apiUrl = $"https://www.thehuxley.com/api/v1/quizzes/{codigoTarefa}/problems?max=100&offset=0";

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

        [HttpGet("{codigoTarefa}/{codigoUsuario}/problems")]
        public async Task<IActionResult> GetUsersByQuiz(string codigoTarefa, string codigoUsuario)
        {
            try
            {
                string apiUrl = $"https://www.thehuxley.com/api/v1/quizzes/{codigoTarefa}/users/{codigoUsuario}/problems";

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
                    return Content(responseBody, "application/json"); ;
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
