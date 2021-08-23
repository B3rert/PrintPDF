using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrintPDF.Models;

namespace PrintPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string FileToRead = "";


            //Impresion de pdf, limite de 10 paginas por archivo.

            if (args.Length == 0)
            {
                Console.WriteLine("args is null"); // Check for null array
                return;
            }
            else
            {
                //string FileToRead = @"C:\Nueva carpeta\DetailsPrint.json";
                FileToRead = args[0];
            }
            

            //string FileToRead = @"C:\Nueva carpeta\DetailsPrint.json";


            // Creating string array  
            string[] lines = File.ReadAllLines(FileToRead);
            string text = "";
            text = String.Join(Environment.NewLine, lines);
            PrintConfigModel jsonBody = JsonConvert.DeserializeObject<PrintConfigModel>(text);


            //ruta y nomre del archivo
            string pdfFileName = jsonBody.file_path;
            int pagesPdf = 1;

            //Obtiene el numero de páginas
            using (StreamReader sr = new StreamReader(File.OpenRead(pdfFileName)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());
                pagesPdf =  matches.Count;
            }

            //Obtener numero de paginas del pdf
            for (int i = 0; i < jsonBody.number_prints; i++)
            {
                // Cargar documento PDF
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(pdfFileName);

                // Especifica la impresora
                doc.PrintSettings.PrinterName = jsonBody.printer_name;

                // Establecer el rango de números de página de impresión del documento
                doc.PrintSettings.SelectPageRange(1, pagesPdf);

                // Imprimir documento PDF
                doc.Print();
            }
        }
    }
}