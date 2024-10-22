using Newtonsoft.Json;

namespace Betsson.OnlineWallets.Web.Models
{
    public class ErrorResponse
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("errors")]
        public Dictionary<string, List<string>> Errors { get; set; }

        [JsonProperty("traceId")]
        public string? TraceId { get; set; }
    }
}
