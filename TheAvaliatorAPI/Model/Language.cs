using Newtonsoft.Json;

namespace TheAvaliatorAPI.Model
{
    public class Language
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("compiler")]
        public string Compiler { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("execParams")]
        public object ExecParams { get; set; }

        [JsonProperty("compileParams")]
        public object CompileParams { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
