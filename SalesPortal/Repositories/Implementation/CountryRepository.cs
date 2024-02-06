using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using System.Data;
using System.Data.SqlClient;

namespace SalesPortal.Repositories.Implementation
{
    public class CountryRepository : IGenericRepository<Country>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CountryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SQLConnection");
        }
        public async Task<List<Country>> GetList(Dictionary<string, object> filters)
        {
            List<Country> result = new List<Country>();

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_GetCountries", connection);
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
                        Country country = new Country
                        {
                            CountryID = (int)reader["CountryID"],
                            CountryName = reader["CountryName"].ToString(),
                        };

                        result.Add(country);
                    }
                }

            }
            return result;
        }
    }
}
