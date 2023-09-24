using AspNetCoreDemo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Services.Contracts
{
    public interface ISwiftService
    {
        Task<Response<Swift>> CreateAsync(string filePath);

        Task<Response<Swift>> ParseSwiftAsync(string filePath);

        Task<Response<IEnumerable<Swift>>> GetAllAsync();
    }
}
