using Newtonsoft.Json;

namespace TheAvaliatorAPI.Model
{
    public class Submission
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("tries")]
        public int Tries { get; set; }

        [JsonProperty("comment")]
        public object Comment { get; set; }

        [JsonProperty("submissionDate")]
        public DateTime SubmissionDate { get; set; }

        [JsonProperty("evaluation")]
        public string Evaluation { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("testCaseEvaluations")]
        public List<TestCaseEvaluation> TestCaseEvaluations { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("problem")]
        public Problem Problem { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("errorMsg")]
        public string ErrorMessage { get; set; }

        [JsonProperty("codeParts")]
        public List<object> CodeParts { get; set; }
    }
}
