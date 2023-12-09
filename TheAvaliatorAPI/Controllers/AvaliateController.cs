using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
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
        private readonly IRepositorio<AvaliacaoProfessor> _RepositorioProfessor;
        public AvaliateController(ILogger<ProblemController> logger, HttpClient httpClient, IRepositorio<AvaliacaoAlunos> RepositorioAluno, IRepositorio<AvaliacaoProfessor> RepositorioProfessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _RepositorioAluno = RepositorioAluno;
            _RepositorioProfessor = RepositorioProfessor;
        }



        [HttpPost("avaliarsubmissoes")]
        public async Task<ActionResult> PostAvaliarSubmissao([FromBody] Problema request, int idTurma, int idTarefa, string idProfessor)
        {

            return request.Alunos!.Count > 1 ? await _PostAvaliarVariasSubmissoes(request, idTurma, idTarefa) :
                                              await _PostAvaliarUmaSubmissao(request, idTurma, idTarefa, idProfessor);

        }

        private async Task<ActionResult> _PostAvaliarUmaSubmissao(Problema request, int idTurma, int idTarefa, string idProfessor)
        {
            try
            {

                var avaliacao = _RepositorioAluno.Obter(p => p.Solution == request.Alunos![0].Id.ToString() && p.idProfessor == idProfessor)
                                                    .Join(
                                                        _RepositorioProfessor.ObterTodos(),
                                                        aluno => aluno.idProfessor,
                                                        professor => professor.IdProfessor,
                                                        (aluno, professor) => new
                                                        {
                                                            Aluno = aluno,
                                                            Professor = professor
                                                        });

                Console.WriteLine(idProfessor);
                if (!avaliacao.Any())
                {
                    List<AvaliacaoAlunos> response = await _RequisicaoApi(request);

                    AvaliacaoProfessor avaliacaoprofessor = new AvaliacaoProfessor
                    {
                        IdProfessor = idProfessor,
                        Problem = response[1].Problem,
                        Solution = response[1].Solution,
                        IsTeacher = response[1].IsTeacher,
                        CyclomaticComplexity = response[1].CyclomaticComplexity,
                        ExceededLimitCC = response[1].ExceededLimitCC,
                        LinesOfCode = response[1].LinesOfCode,
                        ExceededLimitLOC = response[1].ExceededLimitLOC,
                        LogicalLinesOfCode = response[1].LogicalLinesOfCode,
                        ExceededLimitLLOC = response[1].ExceededLimitLLOC,
                        SourceLinesOfCode = response[1].SourceLinesOfCode,
                        LimitSLOC = response[1].LimitSLOC,
                        FinalScore = response[1].FinalScore
                    };

                    Console.WriteLine(avaliacaoprofessor.IdProfessor);
                    var avaliacaoProfessor = _RepositorioProfessor.Obter(p => p.Problem == request.Id.ToString() && p.IdProfessor == idProfessor);

                    if (!avaliacaoProfessor.Any())
                        _RepositorioProfessor.Adicionar(avaliacaoprofessor);


                    AvaliacaoAlunos avaliacaoAluno = new AvaliacaoAlunos
                    {
                        IdTurma = idTurma,
                        IdTarefa = idTarefa,
                        idProfessor = idProfessor,
                        Problem = response[0].Problem,
                        Solution = response[0].Solution,
                        IsTeacher = response[0].IsTeacher,
                        CyclomaticComplexity = response[0].CyclomaticComplexity,
                        ExceededLimitCC = response[0].ExceededLimitCC,
                        LinesOfCode = response[0].LinesOfCode,
                        ExceededLimitLOC = response[0].ExceededLimitLOC,
                        LogicalLinesOfCode = response[0].LogicalLinesOfCode,
                        ExceededLimitLLOC = response[0].ExceededLimitLLOC,
                        SourceLinesOfCode = response[0].SourceLinesOfCode,
                        LimitSLOC = response[0].LimitSLOC,
                        FinalScore = response[0].FinalScore
                    };


                    _RepositorioAluno.Adicionar(avaliacaoAluno);

                    return Ok(response);
                }

                var responseHttp = avaliacao.ToList(); 
                
                responseHttp[0].Aluno.AvaliacaoProfessor.AvaliacaoAlunos = null;
                AvaliacaoProfessor professor = responseHttp[0].Aluno.AvaliacaoProfessor!;
                responseHttp[0].Aluno.AvaliacaoProfessor = null;
                AvaliacaoAlunos aluno = responseHttp[0].Aluno;

                return Ok(new List<object> { aluno, professor});

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
            string apiUrl = "https://apiavaliadoratheavaliator-62f424805023.herokuapp.com/avaliarsubmissoes";


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