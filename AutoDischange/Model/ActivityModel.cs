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
        public List<string> ListFile { get; set; }
        public string Workbook { get; set; }
        

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
