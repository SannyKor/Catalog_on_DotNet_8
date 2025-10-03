using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    public class Catalog
    {
        protected List<Unit> units;
        public IReadOnlyList<Unit> Units => units;

        protected Storage storage;


        public Catalog(Storage storage)
        {
            this.storage = storage;
            units = storage.LoadUnits();
        }

        public Unit AddUnit(string name, string description, double price, int quantity, Guid userId)
        {
            Unit unit = storage.InsertUnit(name, description, price, quantity, userId);
            units.Add(unit);
            return unit;
        }

        public Unit? GetUnitById(int id)
        {
            return storage.GetUnitById(id);
        }

        public bool RemoveUnit(int id)
        {
            return storage.RemoveUnit(id) && units.Remove(units.Find(u => u.Id == id));
        }


        public List<Unit> FindUnit(string query)
        {
            return storage.FindUnit(query);
        }
        public void UpdateUnit(Unit unit, Guid userId)
        {
            storage.UpdateUnit(unit, userId);
        }
        public List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id)
        {
            return storage.GetUnitQuantityHistory(id);
        }
        public List<Category> GetCategoriesInUnit(int unitId)
        {
            return storage.GetCategoriesInUnit(unitId);
        }
        
    }
}
