using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    [Serializable]
    public class TaxonomyNotFoundException : Exception 
    {
        public string TaxonomyName { get; }

        public TaxonomyNotFoundException() { }

        public TaxonomyNotFoundException(string message)
            : base(message) { }

        public TaxonomyNotFoundException(string message, Exception inner)
            : base(message, inner) { }

        public TaxonomyNotFoundException(string message, string _taxonomyName)
            : this(message)
        {
            TaxonomyName = _taxonomyName;
        }
    }
}
