using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuild.Models
{
    public class MockUser
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public bool isEneable { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
