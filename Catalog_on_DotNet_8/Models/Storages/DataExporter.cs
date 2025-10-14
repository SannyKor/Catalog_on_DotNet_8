using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Catalog_on_DotNet_8.Migrations;
{
    
}

namespace Catalog_on_DotNet
{
    public static class DataExporter
    {
        public static void ExportUnitsToJson(List<Unit> units, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true};
            var json = JsonSerializer.Serialize(units, options);
            File.WriteAllText(filePath, json);
        }
        public static List<Unit> ImportUnitsFromJson(string filePath)
        {
            if(!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");                
                return new List<Unit>();
            }
            string json = File.ReadAllText(filePath);
            var units = JsonSerializer.Deserialize<List<Unit>>(json);
            return units ?? new List<Unit>();
            
        }
        public static void ExportUnitsToCsv(List<Unit> units, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id,Name,Description,Price,Quantity,AddedDate");
            foreach (var u in units)
            {
                sb.AppendLine($"{u.Id},{u.Name}, {u.Description}, {u.Price}, {u.Quantity}, {u.AddedDate}");
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

        }

        public static List<Unit> ImportUnitsFromCsv(string filePath)
        {
            List<Unit> units = new List<Unit>();
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return units;
            }
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            for (int i = 1; i <lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 6) continue;
                units.Add(new Unit(int.TryParse(parts[0], out int id) ? id : 0)
                {
                    Name = parts[1],
                    Description = parts[2],
                    Price = double.TryParse(parts[3], out double price) ? price : 0,
                    Quantity = int.TryParse(parts[4], out int quantity) ? quantity : 0,
                    AddedDate = DateTime.TryParse(parts[5], out DateTime addedDate) ? addedDate : DateTime.Now
                });
            }
            return units;
        }
    }
}
