using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MainModel
    {
        public MainModel(string Id, string Chagetset, string Path)
        {
            this.Id = Id;
            this.Chagetset = Chagetset;
            this.Path = Path;
        }

        public string Id { get; set; }
        public string Chagetset { get; set; }
        public string Path { get; set; }
    }
}
