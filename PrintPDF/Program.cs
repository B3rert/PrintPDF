using Newtonsoft.Json;
using PrintPDF.Models;
using PrintPDF.Utilities;
using Spire.Pdf;
using System;
using System.Collections.Generic;
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
            //Properties.Settings.Default.Reset();

            startService();
        }
    
        private static void startPrint()
        {
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

            string PDFtoPrint = currentDirectory + @"\" + jsonBody.report_name;

            //Descarga el archivo PDF que se 
            Console.Write("Preparando archivo...");

            using (var progress = new ProgressBar())
            {
                for (int i = 0; i <= 100; i++)
                {
                    progress.Report((double)i / 100);
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
                string info_print = $"{pagesPdf}{Environment.NewLine} Copia {i + 1} de {jsonBody.number_prints}{Environment.NewLine}{Environment.NewLine}{jsonBody.document_name}";
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
            DirectoryInfo di = new DirectoryInfo(currentDirectory + @"\");
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

        //Verifica si hay una licencia guardada, el valor por defecto es "licencia"
        private static bool verifylicense()
        {
            string license = Properties.Settings.Default.Licencia;

            if (license == "licencia")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        //Verifica si un string es numerico
        private static bool isNumber(string number)
        {
            if (number.All(char.IsDigit))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //convierte string a entero
        private static int stringToInt(string str_var)
        {
            return Int32.Parse(str_var);
        }
        
        //verificar si un correo es valido
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //Proceso cuenta correntista
        private static void newCuentaCorrentista()
        {
            //buscar cuenta
            String apis = Properties.Settings.Default.ApisLicense;
            string optCuentaCorrentista;
            bool validOptCuentaCorrentista;

            do
            {
                Console.Clear();
                Console.WriteLine("Cuenta correntista requerida, ¿Ya tines una?:\n" +
               "1. Tengo una cuenta.\n" +
               "2. No tengo una cuenta.\n" +
               "3. Cancelar.");

                optCuentaCorrentista = Console.ReadLine();

                if (string.IsNullOrEmpty(optCuentaCorrentista))
                {
                    Console.WriteLine("Opción invalida");
                    Console.ReadKey();
                    validOptCuentaCorrentista = false;
                }
                else
                {
                    if (isNumber(optCuentaCorrentista))
                    {
                        int opt = stringToInt(optCuentaCorrentista);
                        if (opt == 1 || opt == 2 || opt == 3)
                        {
                            validOptCuentaCorrentista = true;
                        }
                        else
                        {
                            Console.WriteLine("Opción invalida");
                            Console.ReadKey();
                            validOptCuentaCorrentista = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Opción invalida");
                        Console.ReadKey();
                        validOptCuentaCorrentista = false;
                    }
                }

            } while (!validOptCuentaCorrentista);

            if (stringToInt(optCuentaCorrentista) == 3)
            {
                return;
            }
            else if (stringToInt(optCuentaCorrentista) == 2)
            {
                
                CuentaCorrentistaParamsModel cuentaParams = new CuentaCorrentistaParamsModel();

                do
                {
                    Console.Clear();    
                    Console.WriteLine("NUEVA CUENTA\n" +
                        "Nombre Completo:");
                    cuentaParams.name = Console.ReadLine();

                    if (string.IsNullOrEmpty(cuentaParams.name))
                    {
                        Console.WriteLine("No se ha ingresado ningun valor");
                        Console.ReadKey();
                    }

                } while (string.IsNullOrEmpty(cuentaParams.name));

                bool validEmal;

                do
                {
                    Console.Clear();
                    Console.WriteLine("NUEVA CUENTA\n" +
                        "Nombre Completo: " + cuentaParams.name +
                        "\nCorreo electrónico:");
                    cuentaParams.email = Console.ReadLine();

                    if (string.IsNullOrEmpty(cuentaParams.email))
                    {
                        Console.WriteLine("No se ha ingresado ningun valor");
                        Console.ReadKey();
                        validEmal = false;
                    }else if (!IsValidEmail(cuentaParams.email))
                    {
                        Console.WriteLine("Correo electronico invalido.");
                        Console.ReadKey();
                        validEmal = false;

                    }else
                    {
                        validEmal = true;
                    }

                } while (!validEmal);

                bool validPhone;

                do
                {
                    Console.Clear();
                    Console.WriteLine("NUEVA CUENTA\n" +
                        "Nombre Completo: " + cuentaParams.name +
                        "\nCorreo electrónico: " + cuentaParams.email +
                        "\nTeléfono:");
                    cuentaParams.phone = Console.ReadLine();

                    if (string.IsNullOrEmpty(cuentaParams.phone))
                    {
                        Console.WriteLine("No se ha ingresado ningun valor");
                        Console.ReadKey();
                        validPhone = false;
                    }
                    else if (!isNumber(cuentaParams.phone))
                    {
                        Console.WriteLine("Numero telefonico invalido.");
                        Console.ReadKey();
                        validPhone = false;
                    }
                    else
                    {
                        validPhone = true;
                    }

                } while (!validPhone);

                do
                {
                    Console.Clear();
                    Console.WriteLine("NUEVA CUENTA\n" +
                        "Nombre Completo: " + cuentaParams.name +
                        "\nCorreo electrónico: " + cuentaParams.email +
                        "\nTeléfono: "+ cuentaParams.phone +
                        "\nDirección: ");
                    cuentaParams.adress = Console.ReadLine();

                    if (string.IsNullOrEmpty(cuentaParams.adress))
                    {
                        Console.WriteLine("No se ha ingresado ningun valor");
                        Console.ReadKey();
                    }

                } while (string.IsNullOrEmpty(cuentaParams.adress));

                cuentaParams.country = 69;

                //crear cuenta correntista
                var urlPostCuentaCorrentista = $"{apis}CuentaCorrentista";
                var requestPostCuentaCorrentista = (HttpWebRequest)WebRequest.Create(urlPostCuentaCorrentista);
                string jsonPostCuentaCorrentista = JsonConvert.SerializeObject(cuentaParams);
                requestPostCuentaCorrentista.Method = "POST";
                requestPostCuentaCorrentista.ContentType = "application/json";
                requestPostCuentaCorrentista.Accept = "application/json";
                
                using (var streamWriterPostCuentaCorrentista = new StreamWriter(requestPostCuentaCorrentista.GetRequestStream()))
                {
                    streamWriterPostCuentaCorrentista.Write(jsonPostCuentaCorrentista);
                    streamWriterPostCuentaCorrentista.Flush();
                    streamWriterPostCuentaCorrentista.Close();
                }
                try
                {
                    using (WebResponse responsePostCuentaCorrentista = requestPostCuentaCorrentista.GetResponse())
                    {
                        using (Stream strReaderPostCuentaCorrentista = responsePostCuentaCorrentista.GetResponseStream())
                        {
                            if (strReaderPostCuentaCorrentista == null) return;
                            using (StreamReader objReaderrPostCuentaCorrentista = new StreamReader(strReaderPostCuentaCorrentista))
                            {
                                string responseBodyPostCuentaCorrentista = objReaderrPostCuentaCorrentista.ReadToEnd();
                               
                                ResLicenseModel resJsonPost = new ResLicenseModel();

                                resJsonPost = JsonConvert.DeserializeObject<ResLicenseModel>(responseBodyPostCuentaCorrentista);

                                if (resJsonPost.status == 1)
                                {
                                    //crear cuenta cuenta
                                    CtaCtaModel ctaCta = new CtaCtaModel()
                                    {
                                        cuenta_Correntista = stringToInt(resJsonPost.uuiid),
                                        descripcion = cuentaParams.name,
                                        direccion_1 = cuentaParams.adress
                                    };

                                    var urlPostCtaCta = $"{apis}CuentaCta";
                                    var requestostCtaCta = (HttpWebRequest)WebRequest.Create(urlPostCtaCta);
                                    string jsonPostCtaCta = JsonConvert.SerializeObject(ctaCta);
                                    requestostCtaCta.Method = "POST";
                                    requestostCtaCta.ContentType = "application/json";
                                    requestostCtaCta.Accept = "application/json";
                                    
                                    using (var streamWriterPostCtaCta = new StreamWriter(requestostCtaCta.GetRequestStream()))
                                    {
                                        streamWriterPostCtaCta.Write(jsonPostCtaCta);
                                        streamWriterPostCtaCta.Flush();
                                        streamWriterPostCtaCta.Close();
                                    }
                                    try
                                    {
                                        using (WebResponse responsePostCtaCta = requestostCtaCta.GetResponse())
                                        {
                                            using (Stream strReaderPostCtaCta = responsePostCtaCta.GetResponseStream())
                                            {
                                                if (strReaderPostCtaCta == null) return;
                                                using (StreamReader objReaderPostCtaCta = new StreamReader(strReaderPostCtaCta))
                                                {
                                                    string responseBodyPostCtaCta = objReaderPostCtaCta.ReadToEnd();
                                                    // Do something with responseBody
                                                    ResCtaCtaModel resCtaCta = new ResCtaCtaModel();
                                                    resCtaCta = JsonConvert.DeserializeObject<ResCtaCtaModel>(responseBodyPostCtaCta);

                                                    if (resCtaCta.res)
                                                    {
                                                        //Crear licencia

                                                        LicenseModel paramsLicense = new LicenseModel()
                                                        {
                                                            empresa_L = 1,
                                                            cuenta_correntista = stringToInt(resJsonPost.uuiid),
                                                            cuenta_cta = resCtaCta.message,
                                                            application = 1,
                                                            fecha_Vencimiento = getDateNextYear(),
                                                            orden = 1,
                                                            userName = cuentaParams.name

                                                        };

                                                        var urlPostLicense = $"{apis}License";
                                                        var requestPostLicense = (HttpWebRequest)WebRequest.Create(urlPostLicense);
                                                        string json = JsonConvert.SerializeObject(paramsLicense);
                                                        requestPostLicense.Method = "POST";
                                                        requestPostLicense.ContentType = "application/json";
                                                        requestPostLicense.Accept = "application/json";
                                                        
                                                        using (var streamWriter = new StreamWriter(requestPostLicense.GetRequestStream()))
                                                        {
                                                            streamWriter.Write(json);
                                                            streamWriter.Flush();
                                                            streamWriter.Close();
                                                        }
                                                        try
                                                        {
                                                            using (WebResponse responsePostLicense = requestPostLicense.GetResponse())
                                                            {
                                                                using (Stream strReaderpostLicense = responsePostLicense.GetResponseStream())
                                                                {
                                                                    if (strReaderpostLicense == null) return;
                                                                    using (StreamReader objReaderPostLicense = new StreamReader(strReaderpostLicense))
                                                                    {
                                                                        string responseBodyPostlicense = objReaderPostLicense.ReadToEnd();
                                                                        // Do something with responseBody

                                                                        ResLicenseModel resJson = new ResLicenseModel();

                                                                        resJson = JsonConvert.DeserializeObject<ResLicenseModel>(responseBodyPostlicense);

                                                                        if (resJson.status == 1)
                                                                        {
                                                                            Properties.Settings.Default.Licencia = resJson.uuiid;
                                                                            Properties.Settings.Default.Save();
                                                                            Console.WriteLine("La compra de la licencia se realizó correctamente, espere la confirmacion de su pago y vuelva a intentar usar el producto.");
                                                                            Console.ReadKey();
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Algo salió mal: " + resJson.uuiid);
                                                                            Console.ReadKey();
                                                                            newCuentaCorrentista();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (WebException ex)
                                                        {
                                                            // Handle error
                                                            Console.WriteLine("Algo Salió mal " + ex.Message);
                                                            Console.ReadKey();
                                                            newCuentaCorrentista();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("No se pudo asociar la cuenta correntista a una cuenta cuenta: " + resCtaCta.message);
                                                        Console.ReadKey();
                                                        newCuentaCorrentista();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (WebException ex)
                                    {
                                        Console.WriteLine("No se pudo asociar la cuenta correntista a una cuenta cuenta: "+ ex.Message);
                                        Console.ReadKey();
                                        newCuentaCorrentista();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Algo salió mal: " + resJsonPost.uuiid);
                                    Console.ReadKey();
                                    newCuentaCorrentista();
                                }
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Algo salió mal: " + ex.Message);
                    Console.ReadKey();
                    newCuentaCorrentista();
                    // Handle error
                }
            }
            else if (stringToInt(optCuentaCorrentista) == 1)
            {
                string correo;
                bool validEmail;
                do
                {
                    Console.Clear();
                    Console.WriteLine("BUSCAR CUENTA\n" +
                    "Correo electronico:");
                    correo = Console.ReadLine();

                    if (string.IsNullOrEmpty(correo))
                    {
                        Console.WriteLine("No se ha ingresado ningun correo.");
                        Console.ReadKey();
                        validEmail = false;
                    }
                    else if (IsValidEmail(correo))
                    {
                        validEmail = true;
                    }
                    else
                    {
                        Console.WriteLine($"El correo {correo} no es válido.");
                        Console.ReadKey();
                        validEmail = false;
                    }

                } while (!validEmail);

                if (validEmail)
                {
                    var url = $"{apis}CuentaCorrentista/{correo}";
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";
                    try
                    {
                        using (WebResponse response = request.GetResponse())
                        {
                            using (Stream strReader = response.GetResponseStream())
                            {
                                if (strReader == null) return;
                                using (StreamReader objReader = new StreamReader(strReader))
                                {
                                    string responseBody = objReader.ReadToEnd();
                                    // Do something with responseBody

                                    List<CuentaCorrentistaModel> myDeserializedCuentas =
                                        new List<CuentaCorrentistaModel>();

                                    myDeserializedCuentas =
                                        JsonConvert.DeserializeObject<List<CuentaCorrentistaModel>>(responseBody);

                                    if (myDeserializedCuentas.Count == 0)
                                    {
                                        Console.WriteLine("No hay ninguna cuneta asociada a este correo");
                                        Console.ReadKey();
                                        newCuentaCorrentista();
                                    }
                                    else
                                    {
                                        LicenseModel paramsLicense = new LicenseModel()
                                        {
                                            empresa_L = 1,
                                            cuenta_correntista = myDeserializedCuentas[0].cuenta_Correntista,
                                            cuenta_cta = "1",
                                            application = 1,
                                            fecha_Vencimiento = getDateNextYear(),
                                            orden = 1,
                                            userName = myDeserializedCuentas[0].factura_Nombre
                                        };

                                        var urlPostLicense = $"{apis}License";
                                        var requestPostLicense = (HttpWebRequest)WebRequest.Create(urlPostLicense);
                                        string json =JsonConvert.SerializeObject(paramsLicense);
                                        requestPostLicense.Method = "POST";
                                        requestPostLicense.ContentType = "application/json";
                                        requestPostLicense.Accept = "application/json";
                                        
                                        using (var streamWriter = new StreamWriter(requestPostLicense.GetRequestStream()))
                                        {
                                            streamWriter.Write(json);
                                            streamWriter.Flush();
                                            streamWriter.Close();
                                        }
                                        try
                                        {
                                            using (WebResponse responsePostLicense = requestPostLicense.GetResponse())
                                            {
                                                using (Stream strReaderpostLicense = responsePostLicense.GetResponseStream())
                                                {
                                                    if (strReaderpostLicense == null) return;
                                                    using (StreamReader objReaderPostLicense = new StreamReader(strReaderpostLicense))
                                                    {
                                                        string responseBodyPostlicense = objReaderPostLicense.ReadToEnd();
                                                        // Do something with responseBody

                                                        ResLicenseModel resJson = new ResLicenseModel();

                                                        resJson = JsonConvert.DeserializeObject<ResLicenseModel>(responseBodyPostlicense);

                                                        if (resJson.status == 1)
                                                        {
                                                            Properties.Settings.Default.Licencia = resJson.uuiid;
                                                            Properties.Settings.Default.Save();
                                                            Console.WriteLine("La compra de la licencia se realizó correctamente, espere la confirmacion de su pago y vuelva a intentar usar el producto.");
                                                            Console.ReadKey();
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Algo salió mal: "+ resJson.uuiid);
                                                            Console.ReadKey();
                                                            newCuentaCorrentista();

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (WebException ex)
                                        {
                                            // Handle error
                                            Console.WriteLine("Algo Salió mal " + ex.Message);
                                            Console.ReadKey();
                                            newCuentaCorrentista();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        // Handle error
                        Console.WriteLine("Aglo salio mal: " + ex.Message);
                        Console.ReadKey();
                        newCuentaCorrentista();
                    }
                }
            }
        }

        //retorna la fecha que será en un año
        private static string getDateNextYear()
        {
            DateTime dateTime = DateTime.Now;

            int year = dateTime.Year;
            string month = dateTime.Month.ToString();
            string day = dateTime.Day.ToString();

            year++;
            
            if (day.Length == 1)
            {
                day = $"0{day}";
            }

            if (month.Length == 1)
            {
                month = $"0{month}";
            }
            
            return $"{year}-{month}-{day}T18:00:44.206Z";
        }

        private static void startService()
        {
            if (verifylicense())
            {
                string license = Properties.Settings.Default.Licencia;
                string apis = Properties.Settings.Default.ApisLicense;

                var url = $"{apis}License/{license}";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream strReader = response.GetResponseStream())
                        {
                            if (strReader == null) return;
                            using (StreamReader objReader = new StreamReader(strReader))
                            {
                                string responseBody = objReader.ReadToEnd();

                                List<LicenseListModel> licenses = new List<LicenseListModel>();

                                licenses = JsonConvert.DeserializeObject<List<LicenseListModel>>(responseBody);

                                if (licenses.Count == 0)
                                {
                                    Console.WriteLine("No se ha encontrado la licencia guardada en este dispositivo");
                                    Console.ReadKey();
                                }
                                else if (licenses.Count > 0)
                                {
                                    if (licenses[0].estado == 3)
                                    {
                                        Console.WriteLine("La licencia no se encunetra activa o ya venció, contactanos para poder ayudarte");
                                        Console.ReadKey();
                                    }
                                    else if (licenses[0].estado == 1)
                                    {
                                        if (DateTime.Now.Date > licenses[0].fecha_Vencimiento.Date)
                                        {
                                            Console.WriteLine("La licencia no se encunetra activa o ya venció, contactanos para poder ayudarte");
                                            Console.ReadKey();
                                        }
                                        else
                                        {
                                            startPrint();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Algo salio mal: " + ex.Message);
                    Console.ReadKey();
                }
            }
            else
            {
                bool validOptBuyLicense;
                string optBuyLicense;
                do
                {
                    Console.Clear();
                    Console.WriteLine("El producto no está activo, por favor adquiera una licencia.\n" +
                    "1. Comprar licencia.\n" +
                    "2. Cancelar.");

                    optBuyLicense = Console.ReadLine();

                    if (string.IsNullOrEmpty(optBuyLicense))
                    {
                        Console.WriteLine("Opción invalida");
                        Console.ReadKey();
                        validOptBuyLicense = false;
                    }
                    else
                    {
                        if (isNumber(optBuyLicense))
                        {
                            int opt = stringToInt(optBuyLicense);
                            if (opt == 1 || opt == 2)
                            {
                                validOptBuyLicense = true;

                            }
                            else
                            {
                                Console.WriteLine("Opción invalida");
                                Console.ReadKey();
                                validOptBuyLicense = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Opción invalida");
                            Console.ReadKey();
                            validOptBuyLicense = false;
                        }
                    }

                } while (!validOptBuyLicense);

                if (validOptBuyLicense)
                {
                    if (stringToInt(optBuyLicense) == 2)
                    {
                        return;
                    }
                    else
                    {
                        bool validOptPlan;
                        string optPlan;
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Planes disponibles:\n" +
                           "1. 1 año por 299.99 USD\n" +
                           "2. 1 mes por 49.99 USD\n" +
                           "3. Cancelar");

                            optPlan = Console.ReadLine();

                            if (string.IsNullOrEmpty(optPlan))
                            {
                                Console.WriteLine("Opción invalida");
                                Console.ReadKey();
                                validOptPlan = false;
                            }
                            else
                            {
                                if (isNumber(optPlan))
                                {
                                    int opt = stringToInt(optPlan);
                                    if (opt == 1 || opt == 2 || opt == 3)
                                    {
                                        validOptPlan = true;

                                    }
                                    else
                                    {
                                        Console.WriteLine("Opción invalida");
                                        Console.ReadKey();
                                        validOptPlan = false;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Opción invalida");
                                    Console.ReadKey();
                                    validOptPlan = false;
                                }
                            }

                        } while (!validOptPlan);

                        if (validOptPlan)
                        {

                            if (stringToInt(optPlan) == 3)
                            {
                                return;
                            }
                            else
                            {
                                bool validOptCargoAbono;
                                string optCargoAbono;
                                do
                                {
                                    Console.Clear();
                                    Console.WriteLine("Forma de pago:\n" +
                                   "1. Efectivo\n" +
                                   "2. Cancelar");

                                    optCargoAbono = Console.ReadLine();


                                    if (string.IsNullOrEmpty(optCargoAbono))
                                    {
                                        Console.WriteLine("Opción invalida");
                                        Console.ReadKey();
                                        validOptCargoAbono = false;
                                    }
                                    else
                                    {
                                        if (isNumber(optCargoAbono))
                                        {
                                            int opt = stringToInt(optCargoAbono);
                                            if (opt == 1 || opt == 2)
                                            {
                                                validOptCargoAbono = true;

                                            }
                                            else
                                            {
                                                Console.WriteLine("Opción invalida");
                                                Console.ReadKey();
                                                validOptCargoAbono = false;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Opción invalida");
                                            Console.ReadKey();
                                            validOptCargoAbono = false;
                                        }
                                    }
                                } while (!validOptCargoAbono);

                                if (validOptCargoAbono)
                                {
                                    if (stringToInt(optCargoAbono) == 2)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        newCuentaCorrentista();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}