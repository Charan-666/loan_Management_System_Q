namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ILMPService<T, TId> where T : class
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T?> GetById(TId id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task Delete(TId id);
    }
}
