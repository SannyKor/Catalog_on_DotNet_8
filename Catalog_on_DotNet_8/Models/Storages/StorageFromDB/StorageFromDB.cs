using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Catalog_on_DotNet;

namespace Catalog_on_DotNet
{
    public class StorageFromDB : Storage
    {
        private readonly List<Unit> units = new List<Unit>();
        public StorageFromDB()
        {
            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string createTableCatalogSql = @"
                    CREATE TABLE IF NOT EXISTS units (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        description TEXT,
                        price REAL NOT NULL,
                        quantity INTEGER NOT NULL,
                        added_date TEXT NOT NULL
                        );";

                string createTableQuantityHistory = @"
                    CREATE TABLE IF NOT EXISTS quantity_history (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        unit_id INTEGER NOT NULL,
                        new_quantity INTEGER NOT NULL,
                        change_time TEXT NOT NULL,
                        FOREIGN KEY (unit_id) REFERENCES units(id)
                        );";

                using (var command = new SqliteCommand(createTableCatalogSql, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand(createTableQuantityHistory, connection))
                {
                    command.ExecuteNonQuery();
                }

                EnsureStartId("units", 10000, connection);
            }

        }
        public override void SaveUnits(List<Unit> units)
        {

        }
        public override List<Unit> LoadUnits()
        {


            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM units";
                using (var command = new SqliteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        string description = reader.GetString(reader.GetOrdinal("description"));
                        double price = Convert.ToDouble(reader["price"]);
                        int quantity = Convert.ToInt32(reader["quantity"]);
                        DateTime addedDate = Convert.ToDateTime(reader["added_date"]);

                        Unit unit = new Unit(id)
                        {
                            Name = name,
                            Description = description,
                            Price = price,
                            Quantity = quantity,
                            AddedDate = addedDate
                        };

                        units.Add(unit);
                    }
                }
            }
            return units;
        }
        public override Unit InsertUnit(string name, string description, double price, int quantity)
        {
            int getId;
            DateTime addedDate = DateTime.Now;
            Console.WriteLine($"name: {name}, description: {description}");

            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string insertSql = @"INSERT INTO units (name, description, price, quantity, added_date) 
                                    VALUES (@name, @description, @price, @quantity, @added_date);
                                    ";
                using (var command = new SqliteCommand(insertSql, connection))
                {                    
                    command.Parameters.Add(new SqliteParameter("@name", SqliteType.Text) { Value = name });
                    command.Parameters.Add(new SqliteParameter("@description", SqliteType.Text){Value = description });
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.Add("@added_date", SqliteType.Text).Value = addedDate.ToString();
                    command.ExecuteNonQuery();

                }
                using (var getIdCommand = new SqliteCommand("SELECT last_insert_rowid()", connection))
                {
                    getId = Convert.ToInt32(getIdCommand.ExecuteScalar());
                }
                Unit unit = new Unit(getId)
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    Quantity = quantity,
                    AddedDate = addedDate
                };
                var saveQuantityHistory = new Unit.SaveQuantityChange(unit.Id, unit.Quantity, unit.AddedDate);
                unit.QuantityHistory.Add(saveQuantityHistory);


                string insertSqlChangeQuantity = @"
                                    INSERT INTO quantity_history (unit_id, new_quantity, change_time) 
                                    VALUES (@unit_id, @new_quantity, @change_time)";
                using (var command = new SqliteCommand(insertSqlChangeQuantity, connection))
                {
                    command.Parameters.AddWithValue("@unit_id", saveQuantityHistory.UnitId);
                    command.Parameters.AddWithValue("@new_quantity", saveQuantityHistory.NewUnitQuantity);
                    command.Parameters.Add("@change_time", SqliteType.Text).Value = saveQuantityHistory.DateOfChange;
                    command.ExecuteNonQuery();
                }
                return unit;
            }

        }
        public override Unit GetUnitById(int id)
        {
            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string sqlUnit = @"SELECT * FROM units WHERE id = @id";
                using (var command = new SqliteCommand(sqlUnit, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Unit(Convert.ToInt32(reader["id"]))
                            {
                                Name = Convert.ToString(reader["name"]),
                                Description = Convert.ToString(reader["description"]),
                                Price = Convert.ToDouble(reader["price"]),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                AddedDate = Convert.ToDateTime(reader["added_date"])
                            };
                        }
                    }
                }

            }
            return null;
        }
        public override List<Unit.SaveQuantityChange> GetUnitQuantityHistory(int id)
        {
            List<Unit.SaveQuantityChange> quantityHistory = new List<Unit.SaveQuantityChange>();
            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string sqlUnitQuantityHistory = @"SELECT * FROM quantity_history WHERE  unit_id = @unit_id";
                using (var command = new SqliteCommand(sqlUnitQuantityHistory, connection))
                {
                    command.Parameters.AddWithValue("@unit_id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int quantity = Convert.ToInt32(reader["new_quantity"]);
                            DateTime dateOfChenge = Convert.ToDateTime(reader["change_time"]);
                            var saveQuantity = new Unit.SaveQuantityChange(id, quantity, dateOfChenge);
                            quantityHistory.Add(saveQuantity);
                        }
                        return quantityHistory;
                    }
                }
            }
        }
        public override bool RemoveUnit(int id)
        {
            bool wasDelete;
            bool wasDeletedUnitHistory;
            Unit unit = GetUnitById(id);
            if (unit == null)
            {
                return false;
            }
            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string historySql = @"DELETE FROM quantity_history WHERE unit_id=@id";
                using (var command = new SqliteCommand(historySql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    wasDeletedUnitHistory = command.ExecuteNonQuery() > 0;
                }

                string deleteSql = @"DELETE FROM units WHERE id=@id";
                using (var command = new SqliteCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    wasDelete = command.ExecuteNonQuery() > 0;
                }                
            }
            if (wasDelete && wasDeletedUnitHistory)
                return true;
            else
                return false;
        }
        public override void UpdateUnit(Unit unit)
        {
            var oldUnit = GetUnitById(unit.Id);
            bool wasChangedQuantity = oldUnit.Quantity != unit.Quantity;
            DateTime dateTime = DateTime.Now;
            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string updateUnitSql = @"UPDATE units SET 
                                    name = @name,
                                    description = @description,
                                    price = @price,
                                    quantity = @quantity
                                    WHERE id = @id";
                using (var command = new SqliteCommand(updateUnitSql, connection))
                {
                    command.Parameters.Add("@name", SqliteType.Text).Value = unit.Name;
                    command.Parameters.Add("@description", SqliteType.Text).Value = unit.Description;
                    command.Parameters.AddWithValue("@price", unit.Price);
                    command.Parameters.AddWithValue("@quantity", unit.Quantity);
                    command.Parameters.AddWithValue("@id", unit.Id);

                    command.ExecuteNonQuery();
                }

                if (wasChangedQuantity)
                {
                    string insertSqlChangeQuantity = @"
                                    INSERT INTO quantity_history (unit_id, new_quantity, change_time) 
                                    VALUES (@unit_id, @new_quantity, @change_time)";
                    using (var command = new SqliteCommand(insertSqlChangeQuantity, connection))
                    {
                        command.Parameters.AddWithValue("@unit_id", unit.Id);
                        command.Parameters.AddWithValue("@new_quantity", unit.Quantity);
                        command.Parameters.Add("@change_time", SqliteType.Text).Value = dateTime;
                        command.ExecuteNonQuery();
                    }
                }
            }
            Unit unitInList = units.Find(u => u.Id == unit.Id);
            if (unitInList != null)
            {
                unitInList.Name = unit.Name;
                unitInList.Description = unit.Description;
                unitInList.Price = unit.Price;
                unitInList.Quantity = unit.Quantity;
                if (wasChangedQuantity)
                {
                    var saveQuantityHistory = new Unit.SaveQuantityChange(unit.Id, unit.Quantity, dateTime);
                    unitInList.QuantityHistory.Add(saveQuantityHistory);
                }
            }
        }
        public override List<Unit> FindUnit(string query)
        {
            var foundResults = new List<Unit>();
            using (var connection = Sqlite.GetConnection())
            {
                connection.Open();
                string sql = "SELECT * FROM units WHERE name LIKE @search COLLATE NOCASE";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.Add("@search", SqliteType.Text).Value = "%" + query + "%";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var unit = new Unit(Convert.ToInt32(reader["id"]))
                            {
                                Name = Convert.ToString(reader["name"]),
                                Description = Convert.ToString(reader["description"]),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                Price = Convert.ToDouble(reader["price"])
                            };
                            foundResults.Add(unit);
                        }
                    }
                }
            }
            return foundResults;
        }
        private void EnsureStartId(string tableName, int startFromId, SqliteConnection connection)
        {
            string countSql = $"SELECT COUNT (*) FROM {tableName}";
            using (var countCmd = new SqliteCommand(countSql, connection))
            {
                long count = (long)countCmd.ExecuteScalar();
                if (count == 0)
                {
                    string deleteSql = $"DELETE FROM sqlite_sequence WHERE name='{tableName}'";
                    using (var deleteCmd = new SqliteCommand(deleteSql, connection))
                        deleteCmd.ExecuteNonQuery();

                    string insertSql = $"INSERT INTO sqlite_sequence (name, seq) VALUES ('{tableName}', {startFromId})";
                    using (var insertSmd = new SqliteCommand(insertSql, connection))
                        insertSmd.ExecuteNonQuery();
                }
            }
        }

    }
}
