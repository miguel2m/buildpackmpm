using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    public class ActividadDespliegue
    {
        public string OrdenEjec { get; set; } //Orden de Ejecucion
        public string TipoScrpt { get; set; } //Tipo de Script [DML o DDL]
        public string NombArchv { get; set; }//Nombre del Archiv
        public string NombEsqum { get; set; }//Nombre del Esquema
        public string TipoUsr { get; set; } //Tipo Usuario de Esquema o de Conexion
    }
}
