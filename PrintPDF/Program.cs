using Newtonsoft.Json;
using PrintPDF.Models;
using PrintPDF.Utilities;
using Spire.Pdf;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace PrintPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string FileToRead = @"C:\Nueva carpeta\PrintingDetails.json";
            string PDFtoPrint = @"C:\Nueva carpeta\reporte.pdf";

            // Creating string array  
            string[] lines = File.ReadAllLines(FileToRead);
            string text = "";
            text = String.Join(Environment.NewLine, lines);
            PrintConfigModel jsonBody = JsonConvert.DeserializeObject<PrintConfigModel>(text);

            //ruta y nomre del archivo
            string pdfFileName = jsonBody.url_report;
            int pagesPdf = 1;




            Console.Write("Preparando archivo... ");
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i <= 100; i++) {
				progress.Report((double) i / 100);
				//Thread.Sleep(20);
			}

                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        webClient.DownloadFile(jsonBody.url_report, PDFtoPrint);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Problem: " + ex.Message);
                    }
                }

                //Obtiene el numero de páginas
                using (StreamReader sr = new StreamReader(File.OpenRead(PDFtoPrint)))
                {
                    Regex regex = new Regex(@"/Type\s*/Page[^s]");
                    MatchCollection matches = regex.Matches(sr.ReadToEnd());
                    pagesPdf = matches.Count;
                }
            }
            Console.WriteLine("Listo.");


            /**/
            

            /**/


            for (int i = 0; i < jsonBody.number_prints; i++)
            {
                using (PdfDocument doc = new PdfDocument())
                {
                doc.LoadFromFile(PDFtoPrint);
                doc.PrintSettings.PrinterName = jsonBody.printer_name;
                    doc.PrintSettings.DocumentName = jsonBody.document_name;
                doc.PrintSettings.SelectPageRange(1, pagesPdf);
                doc.Print();
            }
               
            }



            //Eliminacion de l archivo descargado (PDF)
            
            if (File.Exists(PDFtoPrint))
            {
                while (File.Exists(PDFtoPrint))
                {
                    File.Delete(PDFtoPrint);
                }

            }
            
        }

    }
}