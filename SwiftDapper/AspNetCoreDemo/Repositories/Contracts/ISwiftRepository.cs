using AspNetCoreDemo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repositories.Contracts
{
    public interface ISwiftRepository
    {
        Task<Swift> CreateAsync(Swift swift);

        Task<IEnumerable<Swift>> GetAllAsync();
    }
}
