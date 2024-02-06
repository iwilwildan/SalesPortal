using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using System.Data;
using System.Data.SqlClient;

namespace SalesPortal.Repositories.Implementation
{
    public class RegionRepository : IGenericRepository<Region>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public RegionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SQLConnection");
        }
        public async Task<List<Region>> GetList(Dictionary<string, object> filters)
        {
            List<Region> result = new List<Region>();

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_GetRegionsByCountry", connection);
                command.CommandType = CommandType.StoredProcedure;

                //Filters: CountryID
                foreach (var filter in filters)
                {
                    command.Parameters.AddWithValue($"@{filter.Key}", filter.Value);
                }

                connection.Open();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Region Region = new Region
                        {
                            RegionID = (int)reader["RegionID"],
                            RegionName = reader["RegionName"].ToString(),
                        };

                        result.Add(Region);
                    }
                }

            }
            return result;
        }
        
    }
}
