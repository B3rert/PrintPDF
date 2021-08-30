using Newtonsoft.Json;
using PrintPDF.Models;
using PrintPDF.Utilities;
using Spire.Pdf;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PrintPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            

            string FileToRead = currentDirectory + @"\PrintingDetails.json";
           

            // Creating string array  
            string[] lines = File.ReadAllLines(FileToRead);
            string text = "";
            text = String.Join(Environment.NewLine, lines);
            PrintConfigModel jsonBody = JsonConvert.DeserializeObject<PrintConfigModel>(text);

            //ruta y nomre del archivo
            string pdfFileName = jsonBody.url_report;
            int pagesPdf = 1;


            string PDFtoPrint = currentDirectory +@"\"+jsonBody.report_name;

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
                        Console.Clear();
                        Console.WriteLine("Algo salió mal: " + ex.Message);
                        Console.WriteLine("Presione una tecla para Finalizar");
                        Console.ReadKey();
                        return;
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
            for (int i = 0; i < jsonBody.number_prints; i++)
            {
                string info_print = $"{pagesPdf}{Environment.NewLine} Copia {i + 1} de {jsonBody.number_prints}{Environment.NewLine}{Environment.NewLine}{jsonBody.document_name}" ;
               // string info_print = $"{pagesPdf} (Copia {i + 1} de {jsonBody.number_prints})" ;

                using (PdfDocument doc = new PdfDocument())
                {
                    doc.LoadFromFile(PDFtoPrint);
                    doc.PrintSettings.PrinterName = jsonBody.printer_name;
                    doc.PrintSettings.DocumentName = info_print;
                    // doc.PrintSettings.Copies = (short)jsonBody.number_prints; //este metodo es mas rapido que hacer un for o foreach
                    doc.PrintSettings.SelectPageRange(1, pagesPdf);
                    doc.Print();

                }

            }

            //Eliminacion de archivos archivos descargados


            if (File.Exists(PDFtoPrint))
            {
                while (File.Exists(PDFtoPrint))
                {
                    File.Delete(PDFtoPrint);
                }

            }

            if (File.Exists(FileToRead))
            {
                while (File.Exists(FileToRead))
                {
                    File.Delete(FileToRead);
                }

            }


            DirectoryInfo di = new DirectoryInfo(currentDirectory +@"\");
            FileInfo[] files = di.GetFiles("*.dsprint")
                                 .Where(p => p.Extension == ".dsprint").ToArray();
            foreach (FileInfo file in files)
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch { }

        }

    }
}