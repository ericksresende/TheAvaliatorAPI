using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;
using TheAvaliatorAPI.Model;

namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class SubmissionController : ControllerBase
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly HttpClient _httpClient;

        public SubmissionController(ILogger<SubmissionController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("submissions")]
        public async Task<IActionResult> GetSubmissions(int? problem = null, string? evaluations = null, string? submissionDateGe = null, string? submissionDateLe = null)
        {
            try
            {
                string apiUrl = "https://www.thehuxley.com/api/v1/user/submissions";
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                    
                queryString["max"] = "100";
                queryString["offset"] = "0";
                queryString["order"] = "desc";
                queryString["sort"] = "submissionDate";

                if (problem != null)
                    queryString["problem"] = problem.ToString();

                if (!string.IsNullOrEmpty(evaluations))
                    queryString["evaluations"] = evaluations;

                queryString["submissionDateGe"] = submissionDateGe;
                queryString["submissionDateLe"] = submissionDateLe;

                var queryStringValue = queryString.ToString();

                string requestUrl = $"{apiUrl}?{queryStringValue}";
                if (string.IsNullOrEmpty(queryStringValue))
                {
                    requestUrl = $"{apiUrl}?max=100&offset=0&order=desc&sort=submissionDate";
                }

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
                    var submissions = JsonConvert.DeserializeObject<List<Submission>>(responseBody);

                    return Ok(submissions);
                }

                return BadRequest("Erro ao obter submissões do usuário!");
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro ao obter submissões do usuário!");
            }
        }

        [HttpGet("submissions/{id}")]
        public async Task<IActionResult> GetSubmissionById(int id)
        {
            try
            {
                string apiUrl = "https://www.thehuxley.com/api/v1/submissions";
                string requestUrl = $"{apiUrl}/{id}";

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
                    var submissions = JsonConvert.DeserializeObject<Submission>(responseBody);

                    return Ok(submissions);
                }

                return BadRequest("Failed to retrieve submissions");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving submissions");
            }
        }


        [HttpGet("submissions/group/{codigoTurma}/quiz/{codigoTarefa}")]
        public async Task<IActionResult> GetSubmissionById(string codigoTurma, string codigoTarefa)
        {
            try
            {

                string apiUrl = $"https://www.thehuxley.com/api/v1/submissions?group={codigoTurma}&max=10&offset=0&order=desc&quiz={codigoTarefa}&sort=submissionDate";

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
                    var submissions = JsonConvert.DeserializeObject<List<Submission>>(responseBody);

                    return Ok(submissions);
                }

                return BadRequest("Failed to retrieve submissions");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving submissions");
            }
        }
        [HttpGet("submissions/{codigoProblema}/{codigoUsuario}/{dataLimite}")]
        public async Task<IActionResult> GetSubmissionById(string codigoProblema, string codigoUsuario, string dataLimite)
        {
            try
            {

                string apiUrl = $"https://www.thehuxley.com/api/v1/submissions?problem={codigoProblema}&submissionDateLe={dataLimite}&user={codigoUsuario}";

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
                    var submissions = JsonConvert.DeserializeObject<List<Submission>>(responseBody);

                    return Ok(submissions);
                }

                return BadRequest("Failed to retrieve submissions");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving submissions");
            }
        }
        [HttpGet("submissions/sourcecode/{codigoSubmissao}")]
        public async Task<IActionResult> GetSubmissionById(string codigoSubmissao)
        {
            try
            {

                string apiUrl = $"https://www.thehuxley.com/api/v1/submissions/{codigoSubmissao}/sourcecode";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (Request.Headers.TryGetValue("Authorization", out var header))
                {
                    var authHeaderValue = header.ToString();
                    var token = authHeaderValue.Replace("Bearer ", "");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(responseBody);

                return Ok(responseBody);
                
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving submissions");
            }
        }
        [HttpGet("{codigoProblema}/professor")]
        public async Task<IActionResult> GetSubmissionNew(string codigoProblema)
        {
            try
            {

                string apiUrl = $"https://www.thehuxley.com/api/v1/user/problems/{codigoProblema}/submissions?currentPage=1&max=10&offset=0&order=desc&sort=submissionDate&totalItems=0";

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
                    var submissions = JsonConvert.DeserializeObject<List<Submission>>(responseBody);

                    return Ok(submissions);
                }

                return BadRequest("Failed to retrieve submissions");
                
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving submissions");
            }
        }
    }
}
