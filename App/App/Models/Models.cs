using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Paradero
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public List<Micro> Micros { get; set; }
    }

    public class Micro
    {
        public string Servicio { get; set; }
        public string Patente { get; set; }
        public bool PoseeInfo { get; set; }
        public string Mensaje { get; set; }
        public int Distancia { get; set; }
    }
}
