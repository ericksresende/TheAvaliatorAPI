using Newtonsoft.Json;
using TheAvaliatorAPI.Model.ClassesModel;

namespace TheAvaliatorAPI.Model.Quiz
{
    public class Quiz
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [JsonProperty("serverTime")]
        public string ServerTime { get; set; }

        [JsonProperty("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("lastUpdated")]
        public string LastUpdated { get; set; }

        [JsonProperty("group")]
        public Group Group { get; set; }

        [JsonProperty("partialScore")]
        public bool PartialScore { get; set; }
    }
}