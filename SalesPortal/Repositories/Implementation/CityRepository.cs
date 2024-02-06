using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using System.Data;
using System.Data.SqlClient;

namespace SalesPortal.Repositories.Implementation
{
    public class CityRepository : IGenericRepository<City>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CityRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SQLConnection");
        }
        public async Task<List<City>> GetList(Dictionary<string, object> filters)
        {
            List<City> result = new List<City>();

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_GetCitiesByRegion", connection);
                command.CommandType = CommandType.StoredProcedure;

                //Filters: RegionID
                foreach (var filter in filters)
                {
                    command.Parameters.AddWithValue($"@{filter.Key}", filter.Value);
                }

                connection.Open();


                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        City City = new City
                        {
                            CityID = (int)reader["CityID"],
                            CityName = reader["CityName"].ToString(),
                        };

                        result.Add(City);
                    }
                }

            }
            return result;
        }

    }
}
