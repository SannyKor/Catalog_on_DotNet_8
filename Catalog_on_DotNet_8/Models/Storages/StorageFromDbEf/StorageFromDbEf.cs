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
            dbContext.Database.EnsureCreated(); // Ensure the database is created
        }
        public override List<Unit> FindUnit(string query)
        {
            throw new NotImplementedException();
        }

        public override Unit GetUnitById(int id)
        {
            throw new NotImplementedException();
        }

        public override List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id)
        {
            throw new NotImplementedException();
        }

        public override Unit InsertUnit(string name, string description, double price, int quantity)
        {
            throw new NotImplementedException();
        }

        public override List<Unit> LoadUnits()
        {
            throw new NotImplementedException();
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
