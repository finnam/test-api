using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace TestApi.Function
{
    public interface IRestApi
    {
        Task<string> GetEndPointAsync(string ep);
    }
}
