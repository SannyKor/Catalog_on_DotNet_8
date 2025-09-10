using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog_on_DotNet;

namespace Catalog_on_DotNet
{
    public class Unit
    {


        public Unit(int Id)
        {
            this.Id = Id;
        }
        public Unit() { }

        public int Id { get; protected set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public List<SaveQuantityChange> QuantityHistory { get; set; } = new List<SaveQuantityChange>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public class SaveQuantityChange
        {
            public SaveQuantityChange(int unitId, int newUnitQuantity, DateTime dateOfChange, Guid userId)
            {

                UnitId = unitId;
                NewUnitQuantity = newUnitQuantity;
                DateOfChange = dateOfChange;
                UserId = userId;
            }
            public SaveQuantityChange() { }
            public Guid UserId { get; set; }
            public int Id { get; set; }
            public int UnitId { get; protected set; }
            public int NewUnitQuantity { get; protected set; }
            public DateTime DateOfChange { get; protected set; }
        }
    }
}
