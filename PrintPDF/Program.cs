using Newtonsoft.Json;
using PrintPDF.Models;
using PrintPDF.Utilities;
using Spire.Pdf;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PrintPDF
{
    class Program
    {
        //Texto a hexadecimal
        public static string ToHexString(string str)
        {
            string hexOutput = "";
            char[] values = str.ToCharArray();
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                hexOutput += String.Format("{0:X}", value);
            }

            return hexOutput;
        }

        //string a Array Bytes
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        static void Main(string[] args)
        {
            /*
            //Recibir Texto (normal a hexadecimal)
            Console.WriteLine("Texto a Hexadecimal:");
            String strToHex = Console.ReadLine();
            //Converit texto a hexadecimal
            Console.WriteLine(ToHexString(strToHex));
            //recibir texto (hexadecimal a texto normal)
            Console.WriteLine("Hexadecimal a texto:");
            String hexToStr = Console.ReadLine();
            //Convertir texto a byte de array
            byte[] byteStr = StringToByteArray(hexToStr); 
            //Convertir byte de array a hexadecimal
            String s = Encoding.UTF8.GetString(byteStr, 0, byteStr.Length);
            Console.WriteLine(s);
            Console.ReadKey();  

            return;
            */
            /*

            //Buscar datos permannetes

            String valueKey = Properties.Settings.Default.Usuario;

            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString().Replace("-", string.Empty);
            int lengthuuid = myuuidAsString.Length;


            Console.WriteLine("Your UUID is: " + myuuidAsString + " and length "+ lengthuuid);

            if (valueKey != "usuario")
            {
                Console.WriteLine(valueKey);
            }
            else
            {
                Console.WriteLine("No se han enctrodado datos");
            }

            //No hay datos guardados mostar mensajes para activar el producto
            
            Console.WriteLine("Usuario:");
            String user = Console.ReadLine();
            Properties.Settings.Default.Usuario = user;
            Properties.Settings.Default.Save();

            Console.WriteLine("Contraseña:");
            String pass = Console.ReadLine();

            Console.WriteLine("Clave del producto:");
            String key = Console.ReadLine();

            Console.WriteLine( user +" "+ pass + " " + key);
            Console.ReadKey();

            //verficar inicio de sesiona y avtivacion del producto

           
            //Hasta aquí nuevas adiciones

            return;
            */
            /*
            if (File.Exists(FileToRead))
            {
                while (File.Exists(FileToRead))
                {
                    File.Delete(FileToRead);
                }

            }
            */

            //Guardar datos permanentes
            var currentDirectory = Directory.GetCurrentDirectory(); //Ruta donden se encuntra el programa
            string FileToRead = currentDirectory + @"\PrintingDetails.json"; //Archivo JSON con detalles de impresion

            //Leer el archivo JSON y obtener los detalles para imprimir
            string[] lines = File.ReadAllLines(FileToRead);
            string text = "";
            text = String.Join(Environment.NewLine, lines);
            PrintConfigModel jsonBody = JsonConvert.DeserializeObject<PrintConfigModel>(text);

            //Ruta y nomre del archivo
            string pdfFileName = jsonBody.url_report;
            int pagesPdf = 1;

            string PDFtoPrint = currentDirectory +@"\"+jsonBody.report_name;

            //Descarga el archivo PDF que se 
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

            /*
             *Imprime con un for el numero de impresiones requeridas
             *El proceso puede ser mas rápido si se elimina el for y se usa el metodo Copies en PrintSettings
             *Se usó for solo para controlar el dialogo que el usuario ve
             */
            for (int i = 0; i < jsonBody.number_prints; i++)
            {
                //Formato para el dialog de impresion 
                string info_print = $"{pagesPdf}{Environment.NewLine} Copia {i + 1} de {jsonBody.number_prints}{Environment.NewLine}{Environment.NewLine}{jsonBody.document_name}" ;
               // string info_print = $"{pagesPdf} (Copia {i + 1} de {jsonBody.number_prints})" ;

                //Configuracion de la impresion
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

            //Elimina el PDF que usó para la impresion
            if (File.Exists(PDFtoPrint))
            {
                while (File.Exists(PDFtoPrint))
                {
                    File.Delete(PDFtoPrint);
                }

            }
            //Elimina el .json con los detalles de impresion
            if (File.Exists(FileToRead))
            {
                while (File.Exists(FileToRead))
                {
                    File.Delete(FileToRead);
                }

            }
            //Elimina .dsprint, esta extension y archivo 
            //son propios del entorno de producción en el que el proceso fue implemteado.
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