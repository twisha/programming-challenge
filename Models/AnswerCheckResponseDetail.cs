using System.Collections.Generic;

namespace ProgrammingChallenge.Models
{
    public class AnswerCheckResponseDetail
    {
        public string TotalTime { get; set; }
        public bool AnswerCorrect { get; set; }
        public IEnumerable<string> ShouldBe { get; set; }
    }

}
