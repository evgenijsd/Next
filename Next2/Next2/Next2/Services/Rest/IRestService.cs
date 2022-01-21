using Next2.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Rest
{
    public interface IRestService
    {
        Task<T> RequestAsync<T>(HttpMethod httpMethod, string requestUrl, Dictionary<string, string> additionalHeaders = null, object requestBody = null);
    }
}
