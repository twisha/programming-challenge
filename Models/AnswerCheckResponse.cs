using Newtonsoft.Json;
using System;
using System.Text;

namespace ProgrammingChallenge.Models
{
    public class AnswerCheckResponse
    {
        [JsonProperty("data")]
        public AnswerCheckResponseDetail Detail { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
