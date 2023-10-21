using Newtonsoft.Json;

namespace TheAvaliatorAPI.Model
{
    public class Problem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
