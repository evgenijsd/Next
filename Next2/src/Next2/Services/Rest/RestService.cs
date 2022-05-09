using Newtonsoft.Json;
using Next2.Helpers.DTO;
using Next2.Resources.Strings;
using Next2.Services.SettingsService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Rest
{
    public class RestService : IRestService
    {
        private readonly ISettingsManager _settingsManager;

        private TaskCompletionSource<bool> _tokenRefreshingSource;

        public RestService(
            ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        #region -- IRestService implementation --

        public async Task<T> GetAsync<T>(string requestUrl, Dictionary<string, string> additioalHeaders = null)
        {
            using (var response = await MakeRequestAsync(requestUrl, HttpMethod.Get, null, additioalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<T> PutAsync<T>(string requestUrl, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            using (var response = await MakeRequestAsync(requestUrl, HttpMethod.Put, requestBody, additioalHeaders))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<T> DeleteAsync<T>(string requestUrl, Dictionary<string, string> additioalHeaders = null)
        {
            using (var response = await MakeRequestAsync(requestUrl, HttpMethod.Delete, null, additioalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<T> DeleteAsync<T>(string requestUrl, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            using (var response = await MakeRequestAsync(requestUrl, HttpMethod.Get, requestBody, additioalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<T> PostAsync<T>(string requestUrl, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            using (var response = await MakeRequestAsync(requestUrl, HttpMethod.Post, requestBody, additioalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<GetT> PostAsync<PostT, GetT>(string requestUrl, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            using (var response = await MakeRequestAsync(requestUrl, HttpMethod.Post, requestBody, additioalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<GetT>(data);
            }
        }

        #endregion

        #region -- Private helpers --

        private static void ThrowIfNotSuccess(HttpResponseMessage response, object dataObj = null)
        {
            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal string BuildParametersString(Dictionary<string, string> parameters)
        {
            string result;

            if (parameters is null || parameters.Count == 0)
            {
                result = string.Empty;
            }
            else
            {
                var sb = new StringBuilder("?");
                bool needAddDivider = false;

                foreach (var item in parameters)
                {
                    if (needAddDivider)
                    {
                        sb.Append('&');
                    }

                    var encodedKey = WebUtility.UrlEncode(item.Key);
                    var encodedVal = WebUtility.UrlEncode(item.Value);

                    sb.Append($"{encodedKey}={encodedVal}");

                    needAddDivider = true;
                }

                result = sb.ToString();
            }

            return result;
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string requestUrl, HttpMethod method, object requestBody = null, Dictionary<string, string> additioalHeaders = null)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(method, requestUrl);

            if (requestBody is not null)
            {
                var json = JsonConvert.SerializeObject(requestBody);

                if (requestBody is IEnumerable<KeyValuePair<string, string>> body)
                {
                    request.Content = new FormUrlEncodedContent(body);
                }
                else
                {
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }
            }

            if (additioalHeaders is not null)
            {
                foreach (var header in additioalHeaders)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (_settingsManager.IsAuthorizationComplete)
            {
                await RefreshTokenIfNeeded();
            }

            return await client.SendAsync(request).ConfigureAwait(false);
        }

        private async Task RefreshTokenIfNeeded()
        {
            if (_settingsManager.TokenExpirationDate < DateTime.Now)
            {
                await RefreshTokenAsync();
            }
        }

        private Task RefreshTokenAsync()
        {
            Task result;

            if (_tokenRefreshingSource is null || _tokenRefreshingSource.Task.IsCompleted)
            {
                _tokenRefreshingSource = new TaskCompletionSource<bool>();
                result = TryRefreshAccessTokenAsync();
            }
            else
            {
                result = _tokenRefreshingSource.Task;
            }

            return result;
        }

        private async Task TryRefreshAccessTokenAsync()
        {
            var responseBody = new RefreshTokenQuery()
            {
                EmployeeId = _settingsManager.UserId.ToString(),
                Tokens = new Tokens()
                {
                    AccessToken = _settingsManager.Token,
                    RefreshToken = _settingsManager.RefreshToken,
                },
            };

            try
            {
                var resultData = await PostAsync<RefreshTokenQueryResultExecutionResult>($"{Constants.API.HOST_URL}/api/auth/refresh-token", responseBody);

                if (resultData.Success)
                {
                    _settingsManager.Token = resultData.Value.Tokens.AccessToken;
                    _settingsManager.RefreshToken = resultData.Value.Tokens.RefreshToken;
                    _settingsManager.TokenExpirationDate = DateTime.Now.AddHours(Constants.API.TOKEN_EXPIRATION_TIME);

                    _tokenRefreshingSource.TrySetResult(true);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Token refreshed");
#endif
                }
                else
                {
                    _settingsManager.IsAuthorizationComplete = false;
                    _settingsManager.Token = string.Empty;
                    _settingsManager.RefreshToken = string.Empty;
                    _settingsManager.TokenExpirationDate = DateTime.Now;

                    _tokenRefreshingSource.TrySetResult(false);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Refreshing token failed");
#endif
                }
            }
            catch (Exception ex)
            {
                _tokenRefreshingSource.TrySetResult(false);
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Bad Request: {ex.Message}");
#endif
            }
        }

        #endregion
    }
}
