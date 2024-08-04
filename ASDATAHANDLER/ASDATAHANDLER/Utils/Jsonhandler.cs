using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASDATAHANDLER.Utils
{
    public class Jsonhandler
    {
        public static string SerializeObject<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static void SerializeObjectToFile<T>(T obj, string filePath)
        {
            var json = SerializeObject(obj);
            File.WriteAllText(filePath, json);
        }

        public static T DeserializeObjectFromFile<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return DeserializeObject<T>(json);
        }
    }
}
