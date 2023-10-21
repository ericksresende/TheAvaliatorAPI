using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TheAvaliatorAPI.Model;

namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class AvaliateController : ControllerBase
    {
        private readonly ILogger<ProblemController> _logger;
        private readonly HttpClient _httpClient;

        public AvaliateController(ILogger<ProblemController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }



        [HttpPost("avaliarsubmissoes")]
        public async Task<ActionResult> PostAvaliarSubmissao([FromBody] Problema request)
        {
            try
            {
                return await RequisicaoApi(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during avaliate");
                return StatusCode(500, "An error occurred during avaliate");
            }
        }
        public async Task<ActionResult> RequisicaoApi(Problema request)
        {
            string apiUrl = "https://avaliador.guugascode.site/avaliarsubmissoes";


            var jsonContent = new StringContent(JsonConvert.SerializeObject(request),
                                                    Encoding.UTF8,
                                                    "application/json");


            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            Console.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SubmissionAvaliateResponse>>(responseBody);


                return Ok(data);
            }
            else
            {
                return BadRequest("Failed to avaliation");
            }
        }
    }
}