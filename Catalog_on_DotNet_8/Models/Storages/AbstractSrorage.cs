using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog_on_DotNet;

namespace Catalog_on_DotNet
{
    public abstract class Storage
    {
        public abstract void SaveUnits(List<Unit> units);
        public abstract List<Unit> LoadUnits();
        public abstract Unit InsertUnit(string name, string description, double price, int quantity, Guid userId);
        public abstract Unit? GetUnitById(int id);
        public abstract bool RemoveUnit(int id);
        public abstract void UpdateUnit(Unit unit, Guid userId);
        public abstract List<Unit> FindUnit(string query);
        public abstract List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id);
        public abstract List<Category> GetCategoriesInUnit(int unitId);
    }
}
