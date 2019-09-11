using System;
using System.Collections.Generic;
using System.Text;

namespace ProgrammingChallenge.Models
{
    public class Subscriber
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<int> MagazineIds { get; set; }
    }
}
