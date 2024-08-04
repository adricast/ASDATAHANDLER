using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASDATAHANDLER.Models;
using ASDATAHANDLER.Services;
using ASDATAHANDLER.Utils; // Importa el espacio de nombres de tu librería

namespace TESTCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvFilePath = "data.csv";
            string jsonFilePath = "data.json";
            string jsonFilePath2 = "datos.json";


          
           
            // Crear datos de ejemplo con más registros
            /*
            var data = new List<CsvModel>
            {
                new CsvModel(new[] { "ID", "Name", "Age", "Occupation" }),
                new CsvModel(new[] { "1", "John Doe", "30", "Software Engineer" }),
                new CsvModel(new[] { "2", "Jane Smith", "25", "Data Scientist" }),
                new CsvModel(new[] { "3", "Alice Johnson", "35", "Product Manager" }),
                new CsvModel(new[] { "4", "Bob Brown", "28", "UX Designer" }),
                new CsvModel(new[] { "5", "Charlie Davis", "40", "Project Manager" }),
                new CsvModel(new[] { "6", "Diana Evans", "32", "QA Engineer" })
            };
            */
            // Crear una instancia de CsvService
            var csvService = new csv(); // Ajusta según el nombre de tu servicio

            // Escribir el archivo CSV
            /*
            csvService.WriteCsv(csvFilePath, data);

            Console.WriteLine("Archivo CSV creado con éxito con más registros.");
            */

            // Cargar datos desde el archivo JSON

            var loadedDataFromJson = csvService.LoadDataFromJson(jsonFilePath2);
            Console.WriteLine("Datos cargados desde JSON:");
            foreach (var record in loadedDataFromJson)
            {
                Console.WriteLine(string.Join(", ", record.Values));
            }
            // Ejecutar una consulta SQL sobre el archivo CSV
            string query = "SELECT ID, Name, Occupation WHERE Age > 30 and Occupation like '%Project Manager%' order by Name ";
            try
            {
                // Ejecuta la consulta SQL sobre el archivo CSV
                var queryResult = csvService.ExecuteSqlQuery(csvFilePath, query);
                Console.WriteLine("Resultado de la consulta:");
                foreach (var record in queryResult)
                {
                    Console.WriteLine(string.Join(", ", record.Values));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Exportar datos a JSON
            try
            {
                // Exporta el archivo CSV a JSON
                csvService.ExportToJson(csvFilePath, jsonFilePath);
                Console.WriteLine($"Datos exportados a JSON en {jsonFilePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar a JSON: {ex.Message}");
            }
        }
    }
}

