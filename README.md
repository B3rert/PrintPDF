# Impresión de PDF en C# .NET
Impresión de archivos pdf en una impresora especifica usando [Spire.PDF for .NET](https://www.jianshu.com/go-wild?ac=2&url=https%3A%2F%2Fwww.e-iceblue.cn%2FDownloads%2FSpire-PDF-NET.html)

## Como usar
* Restaure los paqutes NuGet 
* Añada un archivo con nombre PrintingDetails.json en el directorio del proyetco
* Ejecute el pryecto

El archivo PrintingDetails.json contiene las especificaciones para imprimir

## Estructura del archivo PrintingDetails.json
```bash
{
 "printer_name":"Bullzip PDF Printer", //Nombre de impresora
 "url_report":"http://alojamiento/pdf-test.pdf", //Enlace del pdf
 "number_prints": 1, //Numero de impresiones
 "document_name":"Factura Electronica", //Nombre visible en la cola de impresion
 "report_name":"3tlyocowmeqflfizsof1qixi102911.pdf" //Nombre del pdf descargado, la extension .pdf es necesaria
}
```
EL programa lee los atributos del archivo JSON, descarga el documento pdf indicado, lo imprime y lo elimina. \
Se usó una version gratuita de **Sipire.PDF**, esto provoca que haya un limite de impresion de 10 páginas por documento.
