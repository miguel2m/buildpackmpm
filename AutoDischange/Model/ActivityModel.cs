using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    public class ActivityModel
    {
        public int Id { get; set; }
        public string UbicacionA { get; set; }
        public string PathA { get; set; }
        public string HashA { get; set; }
        public DateTime FechaA { get; set; }
        public string LenghtA { get; set; }

        public ActivityModel()
        {
            
        }
        
    }

    public class ActivityComponent
    {
        
        public int Id { get; set; }
        public string PathStart { get; set; }
    }
}
