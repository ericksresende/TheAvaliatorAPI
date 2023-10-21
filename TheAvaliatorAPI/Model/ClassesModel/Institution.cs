using Newtonsoft.Json;

namespace TheAvaliatorAPI.Model.ClassesModel
{
    public class Institution
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("acronym")]
        public string Acronym { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
