using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    //Clase principal
    public class ActivityModel
    {
        public int Id { get; set; } //Contador
        public string Workbook { get; set; } //Sheetspread
    }

    //Clase para la lista de componenetes (ListadoAlojables)
    public class ActivityComponentListAlojables:ActivityModel
    {
        public List<string> DischangeComponentName = new List<string>(); //Compoenent name
    }
    //Clase para la lista de componenetes (ListadoConfigurables)
    public class ActivityComponentListConfigurables : ActivityComponentListAlojables
    {
        public string ComponentEnv { get; set; } //Enviroment ( PRE o PRO)
    }
    //Clase para la lista de componenetes (ListadoScripts)
    public class ActivityComponentListScript : ActivityComponentListAlojables
    {
        public string TypeScript { get; set; } //Type Script SQL ( DML , DDL , ETC)
    }

    //Clase para la lista de componenetes (ListadoScripts)
    public class ActivityComponentPrePro : ActivityModel
    {
        //public int IdExect { get; set; } //Orden de ejcucion
        public string PendindActivity { get; set; } //Actividad Pendiente
        public string TypeActivity { get; set; } //Tipo Actividad
        public string Activity{ get; set; } // Actividad PRE o PRO
    }

    //Model For View
    public class ActivityComponent
    {  
        public int Id { get; set; }
        public string PathStart { get; set; }
    }


}
