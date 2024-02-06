
namespace SalesPortal.Repositories.Contract
{
    public interface ISaleRepository<T> : IGenericRepository<T> where T : class
    {
        Task<int> GetTotalPages(Dictionary<string, object> filters);
        Task<bool> Save(T entity);
    }
}
