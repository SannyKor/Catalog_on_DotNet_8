using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    internal class StorageFromDbEf : Storage
    {
        private readonly CatalogDbContext dbContext;
        public StorageFromDbEf()
        {
            dbContext = new CatalogDbContext();            
        }
        public override List<Unit> FindUnit(string query)
        {
            throw new NotImplementedException();
        }

        public override Unit? GetUnitById(int id)
        {
            return dbContext.Units.Find(id);            
        }

        public override List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id)
        {
            List<Unit.SaveQuantityChange> quantityHistory = new List<Unit.SaveQuantityChange>();
            quantityHistory = dbContext.QuantityChanges
                .Where(q => q.UnitId == id)                
                .ToList();
            return quantityHistory;
        }

        public override Unit InsertUnit(string name, string description, double price, int quantity)
        {
            var unit = new Unit
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity,
                AddedDate = DateTime.Now
            };
            var saveQuantityHistory = new Unit.SaveQuantityChange(unit.Id, unit.Quantity, unit.AddedDate);
            unit.QuantityHistory.Add(saveQuantityHistory);

            dbContext.QuantityChanges.Add(saveQuantityHistory);
            dbContext.SaveChangesAsync();

            dbContext.Units.Add(unit);
            dbContext.SaveChangesAsync();
            return unit;
        }

        public override List<Unit> LoadUnits()
        {
            List<Unit> units = dbContext.Units.ToList();
            foreach (var unit in units)
            {
                unit.QuantityHistory = dbContext.QuantityChanges
                    .Where(q => q.UnitId == unit.Id)
                    .ToList();
            }
            return units;
        }

        public override bool RemoveUnit(int id)
        {
            throw new NotImplementedException();
        }

        public override void SaveUnits(List<Unit> units)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUnit(Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
