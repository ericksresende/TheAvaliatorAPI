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
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("serverTime")]
        public DateTime ServerTime { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("group")]
        public Group Group { get; set; }

        [JsonProperty("partialScore")]
        public bool PartialScore { get; set; }
    }
}
