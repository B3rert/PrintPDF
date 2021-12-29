using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintPDF.Models
{
   public class CuentaCorrentistaParamsModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string adress { get; set; }
        public int country { get; set; }
    }
}
