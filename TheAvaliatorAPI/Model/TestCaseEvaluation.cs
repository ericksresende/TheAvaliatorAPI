using Newtonsoft.Json;

namespace TheAvaliatorAPI.Model
{
    public class TestCaseEvaluation
    {
        [JsonProperty("testCaseId")]
        public int TestCaseId { get; set; }

        [JsonProperty("tip")]
        public string Tip { get; set; }

        [JsonProperty("evaluation")]
        public string Evaluation { get; set; }

        [JsonProperty("errorMsg")]
        public string ErrorMessage { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("diff")]
        public object Diff { get; set; }

        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("large")]
        public bool IsLarge { get; set; }
    }
}
