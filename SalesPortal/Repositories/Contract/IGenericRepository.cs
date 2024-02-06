namespace SalesPortal.Repositories.Contract
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetList(Dictionary<string,object> filters);
        
    }
}
