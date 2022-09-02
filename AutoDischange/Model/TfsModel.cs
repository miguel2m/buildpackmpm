using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
   
        public class Item
        {
            public int version { get; set; }
            public int size { get; set; }
            public string hashValue { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }

        public class Value
        {
            public Item item { get; set; }
            public string changeType { get; set; }
        }

        public class TfsModel
        {
            public int count { get; set; }
            public List<Value> value { get; set; }
        }
    
}
