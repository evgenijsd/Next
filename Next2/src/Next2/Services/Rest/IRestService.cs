﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Rest
{
    public interface IRestService
    {
        Task<T> GetAsync<T>(string resource, Dictionary<string, string> additioalHeaders = null);

        Task<T> PutAsync<T>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null);

        Task<T> DeleteAsync<T>(string resource, Dictionary<string, string> additioalHeaders = null);

        Task<T> DeleteAsync<T>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null);

        Task<T> PostAsync<T>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null);

        Task<GetT> PostAsync<PostT, GetT>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null);
    }
}
