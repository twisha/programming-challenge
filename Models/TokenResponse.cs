using System;
using System.Collections.Generic;
using System.Text;

namespace ProgrammingChallenge.Models
{
    public class TokenResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
