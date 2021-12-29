using System;

namespace PrintPDF.Models
{
    public class LicenseModel
    {
        public int empresa_L { get; set; }
        public int cuenta_correntista { get; set; }
        public string cuenta_cta { get; set; }
        public int application { get; set; }
        public string fecha_Vencimiento { get; set; }
        public int orden { get; set; }
        public string userName { get; set; }
    }
}

