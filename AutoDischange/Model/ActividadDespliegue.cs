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
        public string ServerPre { get; set; } //Servidor BD Pre
        public string ServerPro { get; set; } //Servidor BD Pre
        public string InstanciaPre { get; set; } //Base de Datos
        public string InstanciaPro { get; set; } //Base de Datos
        public string EsquemaPre { get; set; } //Esquema
        public string EsquemaPro { get; set; } //Esquema
        public string Puerto { get; set; } //Puerto
        public int IdGroup { get; set; } //Podria servir para agrupar los Scripts


    }    
}
