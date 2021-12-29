using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintPDF.Models
{
    public class LicenseListModel
    {
        public int empresa_L { get; set; }
        public string codigo_L { get; set; }
        public int cuenta_correntista { get; set; }
        public string cuenta_cta { get; set; }
        public int application { get; set; }
        public string codigo_M { get; set; }
        public int estado { get; set; }
        public DateTime fecha_Vencimiento { get; set; }
        public int orden { get; set; }
        public DateTime fecha_Hora { get; set; }
        public string userName { get; set; }
        public object m_Fecha_Hora { get; set; }
        public object m_UserName { get; set; }
        public DateTime fecha_Compra { get; set; }
    }
}
