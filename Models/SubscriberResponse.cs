using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace programming_challenge.Models
{
    public class SubscriberResponse
    {
        [JsonProperty("data")]
        public IEnumerable<Subscriber> Subscribers { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
