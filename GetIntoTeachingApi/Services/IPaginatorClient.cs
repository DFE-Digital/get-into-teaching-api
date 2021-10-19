using System.Threading.Tasks;

namespace GetIntoTeachingApi.Services
{
    public interface IPaginatorClient<T>
    {
        Task<T> NextAsync();
        bool HasNext { get; }
    }
}
