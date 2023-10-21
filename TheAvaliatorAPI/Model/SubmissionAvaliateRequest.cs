using Newtonsoft.Json;
using System.Collections.Generic;

namespace TheAvaliatorAPI.Model
{
    
    public class Exercicio
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("codigo")]
        public string Codigo { get; set; }
    }

    public class Problema
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("alunos")]
        public List<Exercicio>? Alunos { get; set; }

        [JsonProperty("professor")]
        public string? Professor { get; set; }

        [JsonProperty("problema")]
        public string? NomeProblema { get; set; }
    }

    public class Turma
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("alunos")]
        public List<Exercicio> Alunos { get; set; }

        [JsonProperty("professor")]
        public string Professor { get; set; }

        [JsonProperty("problema")]
        public string Problema { get; set; }
    }

}