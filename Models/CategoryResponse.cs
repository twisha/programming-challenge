using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProgrammingChallenge.Models
{
    public class CategoryResponse
    {
        [JsonProperty("data")]
        public IEnumerable<string> Categories { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
