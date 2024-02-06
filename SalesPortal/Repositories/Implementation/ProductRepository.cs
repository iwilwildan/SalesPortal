using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using System.Data;
using System.Data.SqlClient;

namespace SalesPortal.Repositories.Implementation
{
    public class ProductRepository : IGenericRepository<Product>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SQLConnection");
        }
        public async Task<List<Product>> GetList(Dictionary<string, object> filters)
        {
            List<Product> result = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_GetProducts", connection);
                command.CommandType = CommandType.StoredProcedure;

                foreach (var filter in filters)
                {
                    command.Parameters.AddWithValue($"@{filter.Key}", filter.Value);
                }

                connection.Open();


                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Product Product = new Product
                        {
                            ProductID = (int)reader["ProductID"],
                            ProductName = reader["ProductName"].ToString(),
                        };

                        result.Add(Product);
                    }
                }

            }
            return result;
        }
    }
}
