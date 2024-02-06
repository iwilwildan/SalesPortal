using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using System.Data;
using System.Data.SqlClient;

namespace SalesPortal.Repositories.Implementation
{
    public class SaleRepository : ISaleRepository<Sale>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SaleRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SQLConnection");
        }
        public async Task<List<Sale>> GetList(Dictionary<string, object> filters)
        {
            List<Sale> result = new List<Sale>();

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_GetFilteredSales", connection);
                command.CommandType = CommandType.StoredProcedure;

                //Filters: DateTime startDate, DateTime endDate, string selectedCountry, string selectedRegion, string selectedCity, int pageSize, int pageNumber
                foreach (var filter in filters)
                {
                    command.Parameters.AddWithValue($"@{filter.Key}", filter.Value ?? DBNull.Value);
                }

                connection.Open();


                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Sale sale = new Sale
                        {
                            SaleID = (int)reader["SaleID"],
                            CustomerName = reader["CustomerName"].ToString(),
                            CountryName = reader["CountryName"].ToString(),
                            RegionName = reader["RegionName"].ToString(),
                            CityName = reader["CityName"].ToString(),
                            SaleDateTime = (DateTime)reader["SaleDateTime"],
                            ProductName = reader["ProductName"].ToString(),
                            Quantity = (int)reader["Quantity"]
                        };

                        result.Add(sale);
                    }
                }

            }
            return result;
        }

        public async Task<int> GetTotalPages(Dictionary<string, object> filters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("sp_GetTotalSalesPages", connection);
                command.CommandType = CommandType.StoredProcedure;
                //Filters: DateTime startDate, DateTime endDate, string selectedCountry, string selectedRegion, string selectedCity, int pageSize
                foreach (var filter in filters)
                {
                    command.Parameters.AddWithValue($"@{filter.Key}", filter.Value ?? DBNull.Value);
                }
                // Ensure that the stored procedure returns an int
                object result = await command.ExecuteScalarAsync();

                // Cast the result to int
                return result != null ? (int)result : 0; 
            }
        }

        public async Task<bool> Save(Sale entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_AddSale", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue($"@CustomerName", entity.CustomerName);
                command.Parameters.AddWithValue($"@CountryID", entity.CountryID);
                command.Parameters.AddWithValue($"@RegionID", entity.RegionID);
                command.Parameters.AddWithValue($"@CityID", entity.CityID);
                command.Parameters.AddWithValue($"@SaleDateTime", entity.SaleDateTime);
                command.Parameters.AddWithValue($"@ProductID", entity.ProductID);
                command.Parameters.AddWithValue($"@Quantity", entity.Quantity);

                connection.Open();

                int affectedRows = (int)await command.ExecuteScalarAsync();
                if (affectedRows > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
