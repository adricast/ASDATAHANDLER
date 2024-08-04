using ASDATAHANDLER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASDATAHANDLER.Utils;
using System.Xml;
using System.Text.Json;

namespace ASDATAHANDLER.Services
{
    public class csv
    {
        public List<CsvModel> ReadCsv(string filePath)
        {
            var result = new List<CsvModel>();

            if (!FileHelper.FileExists(filePath))
                throw new FileNotFoundException($"El archivo {filePath} no se encontró.");

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    result.Add(new CsvModel(values));
                }
            }
            return result;
        }

        public void WriteCsv(string filePath, List<CsvModel> data)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var record in data)
                {
                    writer.WriteLine(string.Join(",", record.Values));
                }
            }
        }

        public void CreateCsvWithHeaders(string filePath, List<string> headers, List<CsvModel> data)
        {
            using (var writer = new StreamWriter(filePath))
            {
                // Escribir encabezados
                writer.WriteLine(string.Join(",", headers));

                // Escribir datos
                foreach (var record in data)
                {
                    writer.WriteLine(string.Join(",", record.Values));
                }
            }
        }
        public void ExportToJson(string csvFilePath, string jsonFilePath)
        {
            var csvData = ReadCsv(csvFilePath);
            Jsonhandler.SerializeObjectToFile(csvData, jsonFilePath);
        }
        public void ValidateCsv(string filePath, char delimiter = ',')
        {
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string headerLine = reader.ReadLine();
                var headers = headerLine.Split(delimiter);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(delimiter);
                    if (values.Length != headers.Length)
                    {
                        throw new FormatException("El número de columnas no coincide con el encabezado.");
                    }
                }
            }
        }
        public List<CsvModel> ReadCsvWithQuotes(string filePath, char delimiter = ',', char quoteChar = '"')
        {
            var result = new List<CsvModel>();

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = ParseCsvLine(line, delimiter, quoteChar);
                    result.Add(new CsvModel(values));
                }
            }
            return result;
        }

        private string[] ParseCsvLine(string line, char delimiter, char quoteChar)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == quoteChar)
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == delimiter && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
        public void WriteCsvInChunks(string filePath, IEnumerable<CsvModel> data, char delimiter = ',')
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var record in data)
                {
                    writer.WriteLine(string.Join(delimiter, record.Values));
                }
            }
        }
        public List<CsvModel> ExecuteSqlQuery(string filePath, string query)
        {
            var data = ReadCsv(filePath);
            var queryEngine = new SqlEnginer();
            return queryEngine.ExecuteQuery(data, query);
        }

       
        public List<CsvModel> LoadDataFromJson(string filePath)
        {
            try
            {
                var jsonString = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<List<CsvModel>>(jsonString);
                return data ?? new List<CsvModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos desde JSON: {ex.Message}");
                return new List<CsvModel>();
            }
        }

        public List<CsvModel> LoadDataFromXml(string xmlFilePath)
        {
            var bulkLoader = new BulkDataLoader();
            return bulkLoader.LoadFromXml(xmlFilePath);
        }

      
    }
}
