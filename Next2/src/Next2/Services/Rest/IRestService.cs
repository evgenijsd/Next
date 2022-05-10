using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Rest
{
    public interface IRestService
    {
        Task<T> RequestAsync<T>(HttpMethod method, string resource, Dictionary<string, string> additioalHeaders = null, bool isIgnoreRefreshToken = false);

        Task<T> RequestAsync<T>(HttpMethod method, string resource, object requestBody, Dictionary<string, string> additioalHeaders = null, bool isIgnoreRefreshToken = false);
    }
}
