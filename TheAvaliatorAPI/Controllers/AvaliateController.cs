using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using TheAvaliatorAPI.Infra;
using TheAvaliatorAPI.Model;
using TheAvaliatorAPI.Model.Interface;

namespace TheAvaliatorAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class AvaliateController : ControllerBase
    {
        private readonly ILogger<ProblemController> _logger;
        private readonly HttpClient _httpClient;

        private readonly IRepositorio<AvaliacaoAlunos> _RepositorioAluno;
        public AvaliateController(ILogger<ProblemController> logger, HttpClient httpClient, IRepositorio<AvaliacaoAlunos> RepositorioAluno)
        {
            _logger = logger;
            _httpClient = httpClient;
            _RepositorioAluno = RepositorioAluno;
        }



        [HttpPost("avaliarsubmissoes")]
        public async Task<ActionResult> PostAvaliarSubmissao([FromBody] Problema request, int idTurma, int idTarefa)
        {

            return request.Alunos!.Count > 1 ? await _PostAvaliarVariasSubmissoes(request, idTurma, idTarefa) :
                                              await _PostAvaliarUmaSubmissao(request, idTurma, idTarefa);

        }

        private async Task<ActionResult> _PostAvaliarUmaSubmissao(Problema request, int idTurma, int idTarefa)
        {
            try
            {
                var avaliacao = _RepositorioAluno.Obter(p => p.Solution == request.Alunos![0].Id.ToString());

                if (!avaliacao.Any())
                {
                    List<AvaliacaoAlunos> response = await _RequisicaoApi(request); ;

                    response.RemoveAt(0);

                    response[0].IdTarefa = idTarefa;
                    response[0].IdTurma = idTurma;

                    _RepositorioAluno.AdicionarConjunto(response);

                    return Ok(response);
                }

                return Ok(avaliacao);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during avaliate");
                return StatusCode(500, "An error occurred during avaliate");
            }
        }
        private async Task<ActionResult> _PostAvaliarVariasSubmissoes(Problema request, int idTurma, int idTarefa)
        {
            try
            {
                List<AvaliacaoAlunos> response;
                var avaliacoes = _RepositorioAluno.Obter(p => p.IdTarefa == idTarefa && p.IdTurma == idTurma);

                if (!avaliacoes.Any())
                {
                    response = await _RequisicaoApi(request);

                    foreach (var avaliacao in response)
                    {
                        avaliacao.IdTarefa = idTarefa;
                        avaliacao.IdTurma = idTurma;
                    }

                    response.RemoveAt(0);
                    _RepositorioAluno.AdicionarConjunto(response);

                    return Ok(response);
                }


                List<Exercicio> SubmissoesParaAvaliar = _SubmissoesNaoCalculadas(request, avaliacoes.ToList());

                if (SubmissoesParaAvaliar.IsNullOrEmpty())
                {
                    if (avaliacoes.Count() > request.Alunos!.Count)
                    {
                        List<AvaliacaoAlunos> SubmissoesAvaliadas = _SubimssoesDosAlunosCalculadas(request, avaliacoes.ToList());

                        return Ok(SubmissoesAvaliadas);
                    }
                    return Ok(avaliacoes);
                }

                request.Alunos = SubmissoesParaAvaliar;

                response = await _RequisicaoApi(request);

                foreach (var avaliacao in response)
                {
                    avaliacao.IdTarefa = idTarefa;
                    avaliacao.IdTurma = idTurma;
                }

                response.RemoveAt(0);
                _RepositorioAluno.AdicionarConjunto(response);

                avaliacoes.ToList().AddRange(response);

                return Ok(avaliacoes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during avaliate");
                return StatusCode(500, "An error occurred during avaliate");
            }
        }

        private async Task<List<AvaliacaoAlunos>> _RequisicaoApi(Problema request)
        {
            string apiUrl = "https://avaliador.guugascode.site/avaliarsubmissoes";


            var jsonContent = new StringContent(JsonConvert.SerializeObject(request),
                                                    Encoding.UTF8,
                                                    "application/json");


            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<AvaliacaoAlunos>>(responseBody);


                return data!;
            }
            else
            {
                throw new Exception("Recurso não encontrado.");
            }
        }

        private List<Exercicio> _SubmissoesNaoCalculadas(Problema requisicao, List<AvaliacaoAlunos> avaliacoes)
        {
            List<Exercicio> SubmissoesParaAvaliar = new();

            foreach (Exercicio aluno in requisicao.Alunos!)
            {
                if (avaliacoes.Find(avaliacao => avaliacao.Solution == aluno.Id.ToString()) == null)
                {
                    Console.WriteLine("Não encontrou");
                    SubmissoesParaAvaliar.Add(aluno);
                }
            }

            return SubmissoesParaAvaliar;
        }

        private List<AvaliacaoAlunos> _SubimssoesDosAlunosCalculadas(Problema requisicao, List<AvaliacaoAlunos> avaliacoes)
        {
            List<AvaliacaoAlunos> SubmissoesAvaliadas = new();

            foreach (AvaliacaoAlunos aluno in avaliacoes!)
            {
                if (requisicao.Alunos!.Find((item) => item.Id.ToString() == aluno.Solution) != null)
                {
                    SubmissoesAvaliadas.Add(aluno);
                }
            }

            return SubmissoesAvaliadas;
        }
    }

}