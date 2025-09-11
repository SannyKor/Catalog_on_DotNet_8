using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog_on_DotNet;

namespace Catalog_on_DotNet
{
    public class StorageFromFile : Storage
    {
        private const string FileName = "catalog.bin";
        private int count;
        private readonly List<Unit> units = new List<Unit>();
        public override void SaveUnits(List<Unit> units)
        {
            //var json = JsonSerializer.Serialize(units, new JsonSerializerOptions { WriteIndented = true });
            //File.WriteAllText(FileName, json);
            using (FileStream fs = new FileStream(FileName, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8))
                {
                    writer.Write(units.Count);
                    foreach (var unit in units)
                    {
                        writer.Write(unit.Id);
                        writer.Write(unit.Name);
                        writer.Write(unit.Description);
                        writer.Write(unit.Price);
                        writer.Write(unit.Quantity);
                        writer.Write(unit.AddedDate.Ticks);
                        writer.Write(unit.QuantityHistory.Count);
                        foreach (var history in unit.QuantityHistory)
                        {
                            writer.Write(history.UnitId);
                            writer.Write(history.NewUnitQuantity);
                            writer.Write(history.DateOfChange.Ticks);
                            writer.Write(history.UserId.ToByteArray());
                        }
                    }
                }
            }

        }
        public override List<Unit> LoadUnits()
        {

            if (File.Exists(FileName))
            {
                //var json = File.ReadAllText(FileName);
                //units = JsonSerializer.Deserialize<List<Unit>>(json);
                using (FileStream fs = new FileStream(FileName, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            Unit unit = new Unit(reader.ReadInt32())
                            {

                                Name = reader.ReadString(),
                                Description = reader.ReadString(),
                                Price = reader.ReadDouble(),
                                Quantity = reader.ReadInt32(),
                                AddedDate = new DateTime(reader.ReadInt64()),
                            };
                            int historyCount = reader.ReadInt32();
                            for (int j = 0; j < historyCount; j++)
                            {
                                int unitId = reader.ReadInt32();
                                int quantity = reader.ReadInt32();
                                DateTime dateTime = new DateTime(reader.ReadInt64());
                                Guid userId = new Guid(reader.ReadBytes(16));
                                var quantityHistory = new Unit.SaveQuantityChange(unitId, quantity, dateTime, userId);
                                unit.QuantityHistory.Add(quantityHistory);
                            }
                            units.Add(unit);
                        }
                    }
                }
            }
            count = units.Count;
            return units;
        }
        public override Unit InsertUnit(string name, string description, double price, int quantity, Guid userId)
        {
            int id;
            if (count == 0)
                count = id = 10001;
            else
                count = id = count + 1;

            Unit unit = new Unit(id)
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity
            };
            var saveQuantityHistory = new Unit.SaveQuantityChange(unit.Id, unit.Quantity, unit.AddedDate, userId);
            unit.QuantityHistory.Add(saveQuantityHistory);
            return unit;
        }
        public override Unit? GetUnitById(int id)
        {
            Unit unit = units.Find(u => u.Id == id);
            return unit;
        }
        public override bool RemoveUnit(int id)
        {
            Unit unit = GetUnitById(id);
            if (unit == null) return false;
            return true;
        }
        public override void UpdateUnit(Unit unit, Guid userId)
        {
            Unit _unit = GetUnitById(unit.Id);
            int oldQuantity = _unit.Quantity;
            if (_unit == null) return;
            _unit.Name = unit.Name;
            _unit.Description = unit.Description;
            _unit.Price = unit.Price;
            _unit.Quantity = unit.Quantity;
            if (oldQuantity != _unit.Quantity)
            {
                var saveQuantityHistory = new Unit.SaveQuantityChange(_unit.Id, _unit.Quantity, DateTime.Now, userId);
                _unit.QuantityHistory.Add(saveQuantityHistory);
            }
        }
        public override List<Unit> FindUnit(string query)
        {
            var found = units
                    .FindAll(u => u.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(u => u.Id).ToList();
            return found;
        }
        public override List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id)
        {
            Unit unit = GetUnitById(id);
            List<Unit.SaveQuantityChange> quantityHistory = new List<Unit.SaveQuantityChange>();
            if (unit == null) return null;
            foreach (var quantity in unit.QuantityHistory)
            {
                quantityHistory.Add(quantity);
            }
            return quantityHistory;
        }
    }
}
