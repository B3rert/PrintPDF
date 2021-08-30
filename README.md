# Impresión de PDF en C# .NET
Impresión de archivos pdf en una impresora especifica usando [Spire.PDF for .NET](https://www.jianshu.com/go-wild?ac=2&url=https%3A%2F%2Fwww.e-iceblue.cn%2FDownloads%2FSpire-PDF-NET.html). \
El proyecto está hecho para consola.

## Como usar
* Restaure los paqutes NuGet .
* Añada un archivo con nombre PrintingDetails.json en el directorio del proyetco, donde se encuntre el ejecutable.
* Ejecute el pryecto.

El archivo PrintingDetails.json contiene las especificaciones para imprimir.

## Estructura del archivo PrintingDetails.json
```bash
{
 "printer_name":"Bullzip PDF Printer", 
 "url_report":"http://alojamiento/pdf-test.pdf", 
 "number_prints": 1, 
 "document_name":"Factura Electronica", 
 "report_name":"3tlyocowmeqflfizsof1qixi102911.pdf" 
}
```
## Donde:
* printer_name = nombre de la impresora.
* url_report = url del archivo pdf a descargar e imprimir.
* number_prints = numero de impresiones.
* document_name = nombre visible en la cola de impresion.
* report_name = nombre del pdf descargado mientras se ejcuta el programa luego este archivo se elimina.
 
EL programa lee los atributos del archivo JSON, descarga el documento pdf indicado, lo imprime y lo elimina. 

## Notas
* Se usó una version gratuita de **Sipire.PDF**, esto provoca que haya un limite de impresion de 10 páginas por documento.
* No se maneja ninguna intervensión para documentos con más de 10 páginas, por lo que le dará error.
