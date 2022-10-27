using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
   
        public class TfsItem
    {
            public int version { get; set; }
            public int size { get; set; }
            public string hashValue { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }

        public class TfsValue
    {
            public TfsItem item { get; set; }
            public string changeType { get; set; }
        }

        public class TfsModel
        {
            public int count { get; set; }
            public List<TfsValue> value { get; set; }
        }

    
    public class CheckedInBy
    {
        public string displayName { get; set; }
    }

    public class TfsModelDetail
        {
            public CheckedInBy author { get; set; }
            public string comment { get; set; }
            public DateTime createdDate { get; set; }
    }

}
