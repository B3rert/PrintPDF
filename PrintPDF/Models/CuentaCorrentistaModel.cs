using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintPDF.Models
{
  public  class CuentaCorrentistaModel
    {
        public int cuenta_Correntista { get; set; }
        public string factura_NIT { get; set; }
        public string factura_Nombre { get; set; }
        public string factura_Direccion { get; set; }
        public string id_Cuenta { get; set; }
        public string documento_Nombre { get; set; }
        public object observacion_1 { get; set; }
        public string direccion01 { get; set; }
        public string telefono { get; set; }
        public string eMail { get; set; }
        public object nombre_Empresa { get; set; }
        public string fax_Empresa { get; set; }
        public string nombre { get; set; }
        public string cuenta_Cta { get; set; }
        public object zona { get; set; }
        public string descripcion { get; set; }
        public object eMail_1 { get; set; }
        public object eMail_2 { get; set; }
        public object telefono_1 { get; set; }
        public object telefono_2 { get; set; }
        public object extension_1 { get; set; }
        public object extension_2 { get; set; }
        public object contacto_1 { get; set; }
        public object contacto_2 { get; set; }
        public double limite_Credito { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }


    }
}
