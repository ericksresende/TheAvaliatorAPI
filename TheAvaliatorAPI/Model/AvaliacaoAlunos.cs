using Newtonsoft.Json;

namespace TheAvaliatorAPI.Model
{
    
    public class AvaliacaoAlunos
    {

        public int IdAvaliacao { get; set; }
        public string IdSubmissaoProf { get; set; }
        public int IdTurma { get; set; }
        public int IdTarefa { get; set; }

        [JsonProperty("PROBLEM")]
        public string Problem { get; set; }

        [JsonProperty("SOLUTION")]
        public string Solution { get; set; }

        [JsonProperty("IS_TEACHER")]
        public string IsTeacher { get; set; }

        [JsonProperty("CYCLOMATIC_COMPLEXITY")]
        public string CyclomaticComplexity { get; set; }

        [JsonProperty("EXCEEDED_LIMIT_CC")]
        public string ExceededLimitCC { get; set; }

        [JsonProperty("LINES_OF_CODE")]
        public string LinesOfCode { get; set; }

        [JsonProperty("EXCEEDED_LIMIT_LOC")]
        public string ExceededLimitLOC { get; set; }

        [JsonProperty("LOGICAL_LINES_OF_CODE")]
        public string LogicalLinesOfCode { get; set; }

        [JsonProperty("EXCEEDED_LIMIT_LLOC")]
        public string ExceededLimitLLOC { get; set; }

        [JsonProperty("SOURCE_LINES_OF_CODE")]
        public string SourceLinesOfCode { get; set; }

        [JsonProperty("LIMIT_SLOC")]
        public string LimitSLOC { get; set; }

        [JsonProperty("FINAL_SCORE")]
        public string FinalScore { get; set; }

        public AvaliacaoProfessor? avaliacaoProfessor { get; set; }

    }
}