using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    public enum CatalogAction
    {       
        AddUnit,
        UpdateUnit,
        DeleteUnit,
        ChangeUnitInfo,
        ShowUnit,
        ShowAllUnits,
        ShowQuantityHistory,
        FindUnit,
        CategoriesMenu,
        ShowAllCategories,
        AllUnitsInCategory,
        Exit
    }
    public class MenuItem
    {
        
        public string? Title { get; set; }
        public List<UserRole> AllowedRoles { get; set; } = [];
        public CatalogAction Action { get; set; }

        public MenuItem (string title, List<UserRole> allowedRoles, CatalogAction action)
        {
                Title = title;
                AllowedRoles = allowedRoles;
                Action = action;
        }
    }
}
