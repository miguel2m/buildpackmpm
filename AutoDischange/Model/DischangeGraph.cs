using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    
        public class ValueGraph
    {
            //public string @odata.id { get; set; }
            public IList<IList<string>> values { get; set; }
            public string id { get; set; }
            public int index { get; set; }
            public string name { get; set; }
        }

        public class DischangeGraph
        {
            //public string @odata.context { get; set; }
            public IList<ValueGraph> value { get; set; }
        }
}
