using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SalesXZone.Application.Models;
using SalesXZone.Application.Interfaces;
using System.Data;

namespace SalesXZone.Infrastructure.Repositories
{
    /// <summary>
    /// Repository that calls spITEMS stored procedure.
    /// Uses ADO.NET (no DbContext) to execute the proc and map results.
    /// </summary>
public class StoredProcItemRepository : IItemRepository
    {
        private readonly string _connectionString;

        public StoredProcItemRepository(IConfiguration configuration)
        {
            // name must match your appsettings.json connection string key
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddItemAsync(ItemCreateRequest dto, CancellationToken cancellationToken = default)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

            await using var cmd = new SqlCommand("dbo.spITEMS", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required parameters for your procedure
            cmd.Parameters.Add(new SqlParameter("@Action", SqlDbType.VarChar, 10) { Value = "ADD" });

            cmd.Parameters.Add(new SqlParameter("@ITEM_CODE", SqlDbType.NVarChar, 20) { Value = (object)dto.ItemCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ITEM_NAME", SqlDbType.NVarChar, 200) { Value = (object)dto.ItemName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@WHOLESELL_PRICE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = (object)dto.WholesellPrice ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@MRP_PRICE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = (object)dto.MrpPrice ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@QUANTITY", SqlDbType.Int) { Value = dto.Quantity });
            cmd.Parameters.Add(new SqlParameter("@CATEGORY", SqlDbType.NVarChar, 100) { Value = (object)dto.Category ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UNIT", SqlDbType.NVarChar, 50) { Value = (object)dto.Unit ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CATEGORY_ID", SqlDbType.Int) { Value = (object)dto.CategoryId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IS_ACTIVE", SqlDbType.Bit) { Value = dto.IsActive });

            // The proc returns SELECT SCOPE_IDENTITY() AS [NEW_ITEM_ID];
            // We'll execute reader and read the first resultset.
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            // The first result set is the SELECT SCOPE_IDENTITY() row.
            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                // NEW_ITEM_ID may be decimal depending on SCOPE_IDENTITY type so handle possible numeric types
                var val = reader["NEW_ITEM_ID"];
                if (val == DBNull.Value) throw new Exception("Stored procedure did not return new item id.");
                // SCOPE_IDENTITY returns numeric/decimal; convert to int safely
                var newId = Convert.ToInt32(val);
                return newId;
            }

            throw new Exception("Failed to add item (no id returned).");
        }

        public async Task<ItemMasterModel?> GetItemByIdAsync(int itemId, CancellationToken cancellationToken = default)
        {
            await using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

            await using var cmd = new SqlCommand("dbo.spITEMS", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Action", SqlDbType.VarChar, 10) { Value = "GET" });
            cmd.Parameters.Add(new SqlParameter("@ITEM_ID", SqlDbType.Int) { Value = itemId });
            cmd.Parameters.Add(new SqlParameter("@ACTIVE_ONLY", SqlDbType.Bit) { Value = 0 });

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return MapReaderToModel(reader);
            }

            return null;
        }

        public async Task<List<ItemMasterModel>> GetItemsAsync(bool activeOnly = false, CancellationToken cancellationToken = default)
        {
            var items = new List<ItemMasterModel>();

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

            await using var cmd = new SqlCommand("dbo.spITEMS", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // No @ITEM_ID -> proc returns all rows
            cmd.Parameters.Add(new SqlParameter("@Action", SqlDbType.VarChar, 10) { Value = "GET" });
            cmd.Parameters.Add(new SqlParameter("@ACTIVE_ONLY", SqlDbType.Bit) { Value = activeOnly });

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                items.Add(MapReaderToModel(reader));
            }

            return items;
        }


        private ItemMasterModel MapReaderToModel(SqlDataReader r)
        {
            // Use column names exactly as proc SELECT uses them
            var model = new ItemMasterModel
            {
                ItemId = r.GetFieldValue<int>(r.GetOrdinal("ITEM_ID")),
                ItemCode = r.IsDBNull(r.GetOrdinal("ITEM_CODE")) ? null : r.GetString(r.GetOrdinal("ITEM_CODE")),
                ItemName = r.IsDBNull(r.GetOrdinal("ITEM_NAME")) ? null : r.GetString(r.GetOrdinal("ITEM_NAME")),
                WholesellPrice = r.IsDBNull(r.GetOrdinal("WHOLESELL_PRICE")) ? 0m : r.GetDecimal(r.GetOrdinal("WHOLESELL_PRICE")),
                MrpPrice = r.IsDBNull(r.GetOrdinal("MRP_PRICE")) ? 0m : r.GetDecimal(r.GetOrdinal("MRP_PRICE")),
                Quantity = r.IsDBNull(r.GetOrdinal("QUANTITY")) ? 0 : r.GetInt32(r.GetOrdinal("QUANTITY")),
                Category = r.IsDBNull(r.GetOrdinal("CATEGORY")) ? null : r.GetString(r.GetOrdinal("CATEGORY")),
                CategoryId = r.IsDBNull(r.GetOrdinal("CATEGORY_ID")) ? null : (int?)r.GetInt32(r.GetOrdinal("CATEGORY_ID")),
                CategoryName = r.IsDBNull(r.GetOrdinal("CATEGORY_NAME")) ? null : r.GetString(r.GetOrdinal("CATEGORY_NAME")),
                Unit = r.IsDBNull(r.GetOrdinal("UNIT")) ? null : r.GetString(r.GetOrdinal("UNIT")),
                IsActive = r.IsDBNull(r.GetOrdinal("IS_ACTIVE")) ? false : r.GetBoolean(r.GetOrdinal("IS_ACTIVE")),
                CreatedAt = r.IsDBNull(r.GetOrdinal("CREATED_AT")) ? DateTime.MinValue : r.GetDateTime(r.GetOrdinal("CREATED_AT")),
                UpdatedAt = r.IsDBNull(r.GetOrdinal("UPDATED_AT")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("UPDATED_AT")),
            };

            return model;
        }
    }
}