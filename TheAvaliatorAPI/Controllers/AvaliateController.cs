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

        private readonly IRepositorio<AvaliacaoAlunos> _repositorioAluno;
        private readonly IRepositorio<AvaliacaoProfessor> _repositorioProfessor;
        public AvaliateController(ILogger<ProblemController> logger, HttpClient httpClient, IRepositorio<AvaliacaoAlunos> repositorioAluno, IRepositorio<AvaliacaoProfessor> repositorioProfessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _repositorioAluno = repositorioAluno;
            _repositorioProfessor = repositorioProfessor;
        }



        [HttpPost("avaliarsubmissoes")]
        public async Task<ActionResult> PostAvaliarSubmissao([FromBody] Problema request, int idTurma, int idTarefa, string IdSubmissaoProf)
        {
            return request.Alunos!.Count > 1 ? await PostAvaliarVariasSubmissoes(request, idTurma, idTarefa) :
                                              await PostAvaliarUmaSubmissao(request, idTurma, idTarefa, IdSubmissaoProf);
        }

        private async Task<ActionResult> PostAvaliarUmaSubmissao(Problema request, int idTurma, int idTarefa, string IdSubmissaoProf)
        {
            try
            {
                var avaliacao = _repositorioAluno.Obter(p => p.Solution == request.Alunos![0].Id.ToString() && p.IdSubmissaoProf == IdSubmissaoProf)
                                                    .Join(
                                                        _repositorioProfessor.ObterTodos(),
                                                        aluno => aluno.IdSubmissaoProf,
                                                        professor => professor.IdSubmissaoProf,
                                                        (aluno, professor) => new
                                                        {
                                                            Aluno = aluno,
                                                            Professor = professor
                                                        });

                if (!avaliacao.Any())
                {
                    List<AvaliacaoAlunos> response = await RequisicaoApi(request);

                    AvaliacaoProfessor avaliacaoprofessor = new AvaliacaoProfessor
                    {
                        IdSubmissaoProf = IdSubmissaoProf,
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
                        FinalScore = response[1].FinalScore,
                    };
                    var avaliacaoProfessor = _repositorioProfessor.Obter(p => p.IdSubmissaoProf == IdSubmissaoProf);

                    if (!avaliacaoProfessor.Any()){
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine(avaliacaoprofessor.IdSubmissaoProf);
                        Console.WriteLine("--------------------------------------");
                        _repositorioProfessor.Adicionar(avaliacaoprofessor);
                    }

                    AvaliacaoAlunos avaliacaoAluno = new AvaliacaoAlunos
                    {
                        IdTurma = idTurma,
                        IdTarefa = idTarefa,
                        IdSubmissaoProf = IdSubmissaoProf,
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
                        FinalScore = response[0].FinalScore,
                    };
                    _repositorioAluno.Adicionar(avaliacaoAluno);

                    return Ok(response);
                }

                var responseHttp = avaliacao.ToList();

                responseHttp[0].Aluno.avaliacaoProfessor.AvaliacaoAlunos = null;
                AvaliacaoProfessor professor = responseHttp[0].Aluno.avaliacaoProfessor!;
                responseHttp[0].Aluno.avaliacaoProfessor = null;
                AvaliacaoAlunos aluno = responseHttp[0].Aluno;

                return Ok(new List<object> { aluno, professor });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during avaliate");
                return StatusCode(500, "An error occurred during avaliate");
            }
        }

        private async Task<ActionResult> PostAvaliarVariasSubmissoes(Problema request, int idTurma, int idTarefa)
        {
            try
            {
                List<AvaliacaoAlunos> response;
                var avaliacoes = _repositorioAluno.Obter(p => p.IdTarefa == idTarefa && p.IdTurma == idTurma);

                if (!avaliacoes.Any())
                {
                    response = await RequisicaoApi(request);

                    foreach (var avaliacao in response)
                    {
                        avaliacao.IdTarefa = idTarefa;
                        avaliacao.IdTurma = idTurma;
                    }



                    response.RemoveAt(0);
                    _repositorioAluno.AdicionarConjunto(response);

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

                response = await RequisicaoApi(request);

                foreach (var avaliacao in response)
                {
                    avaliacao.IdTarefa = idTarefa;
                    avaliacao.IdTurma = idTurma;
                }

                response.RemoveAt(0);
                _repositorioAluno.AdicionarConjunto(response);

                avaliacoes.ToList().AddRange(response);

                return Ok(avaliacoes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during avaliate");
                return StatusCode(500, "An error occurred during avaliate");
            }
        }

        private async Task<List<AvaliacaoAlunos>> RequisicaoApi(Problema request)
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
                throw new Exception("Recurso não encontrado." + response);
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