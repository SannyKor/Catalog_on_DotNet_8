using Catalog_on_DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>();
        public List<Unit> Units { get; set; } = new List<Unit>();

    }
}
