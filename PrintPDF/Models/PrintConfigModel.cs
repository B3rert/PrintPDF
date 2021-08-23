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
        public string file_path { get; set; }
        public int number_prints { get; set; }
    }
}
