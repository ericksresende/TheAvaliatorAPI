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
        public async Task<ActionResult> PostAvaliarSubmissao([FromBody] Problema request, int idTurma, int idTarefa, string IdSubmissaoProf)
        {

            return request.Alunos!.Count > 1 ? await _PostAvaliarVariasSubmissoes(request, idTurma, idTarefa) :
                                              await _PostAvaliarUmaSubmissao(request, idTurma, idTarefa, IdSubmissaoProf);

        }

        private async Task<ActionResult> _PostAvaliarUmaSubmissao(Problema request, int idTurma, int idTarefa, string IdSubmissaoProf)
        {
            try
            {

                var avaliacao = _RepositorioAluno.Obter(p => p.Solution == request.Alunos![0].Id.ToString() && p.IdSubmissaoProf == IdSubmissaoProf)
                                                    .Join(
                                                        _RepositorioProfessor.ObterTodos(),
                                                        aluno => aluno.IdSubmissaoProf,
                                                        professor => professor.IdSubmissaoProf,
                                                        (aluno, professor) => new
                                                        {
                                                            Aluno = aluno,
                                                            Professor = professor
                                                        });

                if (!avaliacao.Any())
                {

                    List<AvaliacaoAlunos> response = await _RequisicaoApi(request);
                    int indexProfessor = -1;
                    int indexAluno = -1;

                    for (int i = 0; i < 2; i++)
                    {
                        if(response[i].Solution == "professor"){
                            indexProfessor = i;
                        }
                        else {
                            indexAluno = i;
                        }
                    }

                    AvaliacaoProfessor avaliacaoprofessor = new AvaliacaoProfessor
                    {
                        IdSubmissaoProf = IdSubmissaoProf,
                        Problem = response[indexProfessor].Problem,
                        Solution = response[indexProfessor].Solution,
                        IsTeacher = response[indexProfessor].IsTeacher,
                        CyclomaticComplexity = response[indexProfessor].CyclomaticComplexity,
                        ExceededLimitCC = response[indexProfessor].ExceededLimitCC,
                        LinesOfCode = response[indexProfessor].LinesOfCode,
                        ExceededLimitLOC = response[indexProfessor].ExceededLimitLOC,
                        LogicalLinesOfCode = response[indexProfessor].LogicalLinesOfCode,
                        ExceededLimitLLOC = response[indexProfessor].ExceededLimitLLOC,
                        SourceLinesOfCode = response[indexProfessor].SourceLinesOfCode,
                        LimitSLOC = response[indexProfessor].LimitSLOC,
                        FinalScore = response[indexProfessor].FinalScore,
                    };
                    var avaliacaoProfessor = _RepositorioProfessor.Obter(p => p.IdSubmissaoProf == IdSubmissaoProf);

                    if (!avaliacaoProfessor.Any())
                        _RepositorioProfessor.Adicionar(avaliacaoprofessor);

                    AvaliacaoAlunos avaliacaoAluno = new AvaliacaoAlunos
                    {
                        IdTurma = idTurma,
                        IdTarefa = idTarefa,
                        IdSubmissaoProf = IdSubmissaoProf,
                        Problem = response[indexAluno].Problem,
                        Solution = response[indexAluno].Solution,
                        IsTeacher = response[indexAluno].IsTeacher,
                        CyclomaticComplexity = response[indexAluno].CyclomaticComplexity,
                        ExceededLimitCC = response[indexAluno].ExceededLimitCC,
                        LinesOfCode = response[indexAluno].LinesOfCode,
                        ExceededLimitLOC = response[indexAluno].ExceededLimitLOC,
                        LogicalLinesOfCode = response[indexAluno].LogicalLinesOfCode,
                        ExceededLimitLLOC = response[indexAluno].ExceededLimitLLOC,
                        SourceLinesOfCode = response[indexAluno].SourceLinesOfCode,
                        LimitSLOC = response[indexAluno].LimitSLOC,
                        FinalScore = response[indexAluno].FinalScore,
                    };


                    _RepositorioAluno.Adicionar(avaliacaoAluno);



                    return Ok(new List<object> {response[1], response[0] });
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