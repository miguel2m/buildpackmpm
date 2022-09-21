using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    public class DiffCompareModel
    {
        public int Id                  { get; set; }
        public string UbicacionA       { get; set; }
        public string PathA            { get; set; }
        public string HashA               { get; set; }
        public DateTime FechaA         { get; set; }
        public string LenghtA          { get; set; }

        public string UbicacionB       { get; set; }
        public string PathB            { get; set; }
        public string HashB               { get; set; }
        public DateTime FechaB         { get; set; }
        public string LenghtB          { get; set; }

        public int HashResult          { get; set; }
        public int FechaResult    { get; set; }
        public int LenghtResult     { get; set; }

        public DiffCompareModel()
        {

        }
    }
}
