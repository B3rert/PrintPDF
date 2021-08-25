using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintPDF.Models
{
   public class PrintConfigModel
    {
       public string printer_name { get; set; }
        public string url_report { get; set; }
        public int number_prints { get; set; }
        public string document_name { get; set; }
        public string report_name { get; set; }
    }
}
