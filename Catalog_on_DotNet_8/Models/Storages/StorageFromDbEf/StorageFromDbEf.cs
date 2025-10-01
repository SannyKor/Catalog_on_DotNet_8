using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Catalog_on_DotNet
{
    internal class StorageFromDbEf : Storage
    {
        private readonly CatalogDbContext dbContext;
        public StorageFromDbEf(CatalogDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.Database.EnsureCreated();
            if(!dbContext.Units.Any())
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Units'");
                dbContext.Database.ExecuteSqlRaw("INSERT INTO sqlite_sequence (name,seq) VALUES ('Units', 10000)");
            }
        }
        public override List<Unit> FindUnit(string query)
        {
            var foundResults = new List<Unit>();
            if (!string.IsNullOrWhiteSpace(query))
            {
                foundResults = dbContext.Units
                .Where(u => u.Name.ToLower().Contains(query.ToLower()) || u.Description.ToLower().Contains(query.ToLower()))
                .ToList();
            }
            return foundResults;
        }

        public override Unit? GetUnitById(int id)
        {
            var unit = dbContext.Units
                .Include(u => u.QuantityHistory)
                .FirstOrDefault(u => u.Id == id);
            return unit;            
        }

        public override List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id)
        {
            List<Unit.SaveQuantityChange> quantityHistory = new List<Unit.SaveQuantityChange>();
            quantityHistory = dbContext.QuantityChanges
                .Where(q => q.UnitId == id)                
                .ToList();
            return quantityHistory;
        }

        public override Unit InsertUnit(string name, string description, double price, int quantity, Guid userId)
        {
            var unit = new Unit
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity,
                AddedDate = DateTime.Now
            };

            dbContext.Units.Add(unit);
            dbContext.SaveChanges();

            var saveQuantityHistory = new Unit.SaveQuantityChange(unit.Id, unit.Quantity, unit.AddedDate, userId);
            unit.QuantityHistory.Add(saveQuantityHistory);

            dbContext.QuantityChanges.Add(saveQuantityHistory);
            dbContext.SaveChanges();
            
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

            bool wasDelete;
            
            var unit = dbContext.Units.Find(id);
            if (unit != null)
            {
                dbContext.Units.Remove(unit);
                
                return wasDelete = dbContext.SaveChanges()>0;
            }
            else
            {
                return false;
            }
        }

        public override void SaveUnits(List<Unit> units)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUnit(Unit unit, Guid userId)
        {
            var oldUnit = GetUnitById(unit.Id);
            bool wasChangedQuantity;
            if (oldUnit != null)
            {
                wasChangedQuantity = oldUnit.Quantity != unit.Quantity;
                oldUnit.Name = unit.Name;
                oldUnit.Description = unit.Description;
                oldUnit.Price = unit.Price;
                oldUnit.Quantity = unit.Quantity;
                oldUnit.AddedDate = unit.AddedDate;
                if (wasChangedQuantity)
                {
                    var saveQuantityHistory = new Unit.SaveQuantityChange(unit.Id, unit.Quantity, DateTime.Now, userId);
                    oldUnit.QuantityHistory.Add(saveQuantityHistory);
                    dbContext.QuantityChanges.Add(saveQuantityHistory);
                }

                dbContext.SaveChanges();
            }

        }
       
    }
}
