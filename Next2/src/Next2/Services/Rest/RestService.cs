using Newtonsoft.Json;
using Next2.Models.API;
using Next2.Models.API.Queries;
using Next2.Models.API.Results;
using Next2.Services.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Rest
{
    public class RestService : IRestService
    {
        private readonly ISettingsManager _settingsManager;

        private TaskCompletionSource<bool> _tokenRefreshingSource;

        public RestService(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        #region -- IRestService implementation --

        public async Task<T> RequestAsync<T>(HttpMethod method, string requestUrl, Dictionary<string, string> additionalHeaders = null, bool isIgnoreRefreshToken = false)
        {
            if (_settingsManager.IsAuthorizationComplete && !isIgnoreRefreshToken)
            {
                await RefreshTokenIfNeeded();

                additionalHeaders = GenerateAuthorizationHeader(additionalHeaders);
            }

            using (var response = await MakeRequestAsync(method, requestUrl, null, additionalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<T> RequestAsync<T>(HttpMethod method, string requestUrl, object requestBody, Dictionary<string, string> additionalHeaders = null, bool isIgnoreRefreshToken = false)
        {
            if (_settingsManager.IsAuthorizationComplete && !isIgnoreRefreshToken)
            {
                await RefreshTokenIfNeeded();

                additionalHeaders = GenerateAuthorizationHeader(additionalHeaders);
            }

            using (var response = await MakeRequestAsync(method, requestUrl, requestBody, additionalHeaders).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        #endregion

        #region -- Private helpers --

        private static void ThrowIfNotSuccess(HttpResponseMessage response, object dataObj = null)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(HttpMethod method, string requestUrl, object requestBody = null, Dictionary<string, string> additioalHeaders = null)
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

            return await client.SendAsync(request).ConfigureAwait(false);
        }

        private Task RefreshTokenIfNeeded()
        {
            return _settingsManager.TokenExpirationDate < DateTime.Now ? RefreshTokenAsync() : Task.CompletedTask;
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
                var query = $"{Constants.API.HOST_URL}/api/auth/refresh-token";

                var resultData = await RequestAsync<GenericExecutionResult<RefreshTokenQuery>>(HttpMethod.Post, query, responseBody, null, true);

                if (resultData.Success && resultData.Value is not null)
                {
                    var tokens = resultData.Value.Tokens;

                    _settingsManager.Token = tokens.AccessToken;
                    _settingsManager.RefreshToken = tokens.RefreshToken;
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

        private Dictionary<string, string> GenerateAuthorizationHeader(Dictionary<string, string> additionalHeaders)
        {
            additionalHeaders ??= new();

            additionalHeaders.Add("Authorization", $"Bearer {_settingsManager.Token}");

            return additionalHeaders;
        }

        #endregion
    }
}
