using ASDATAHANDLER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ASDATAHANDLER.Utils
{
    public class BulkDataLoader
    {
        public List<CsvModel> LoadFromJson(string jsonFilePath)
        {
            try
            {
                var jsonData = File.ReadAllText(jsonFilePath);
                var data = Jsonhandler.DeserializeObject<List<CsvModel>>(jsonData);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos desde JSON: {ex.Message}");
                return new List<CsvModel>();
            }
        }
        public List<CsvModel> LoadFromXml(string xmlFilePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CsvModel>));
                using (var stream = new FileStream(xmlFilePath, FileMode.Open))
                {
                    var data = (List<CsvModel>)serializer.Deserialize(stream);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos desde XML: {ex.Message}");
                return new List<CsvModel>();
            }
        }
    }
}
