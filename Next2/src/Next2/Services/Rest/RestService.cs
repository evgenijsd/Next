using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Next2.Helpers.DTO;
using Next2.Resources.Strings;
using Next2.Services.SettingsService;
using System;
using System.Collections.Generic;
using System.IO;
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

        private Dictionary<string, string> _propertyNames = new Dictionary<string, string>
        {
            { nameof(MembershipModelDTO), "memberships" },
            { nameof(CustomerModelDTO), "customers" },
        };

        public RestService(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        #region -- IRestService implementation --

        public async Task<T> RequestAsync<T>(HttpMethod method, string requestUrl, Dictionary<string, string> additionalHeaders = null, bool isIgnoreRefreshToken = false)
        {
            additionalHeaders = GenerateAuthorizationHeader(additionalHeaders);

            using (var response = await MakeRequestAsync(method, requestUrl, null, additionalHeaders, isIgnoreRefreshToken).ConfigureAwait(false))
            {
                ThrowIfNotSuccess(response);

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (method == HttpMethod.Get)
                {
                    data = RenameDataProperty<T>(data);
                }

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        public async Task<T> RequestAsync<T>(HttpMethod method, string requestUrl, object requestBody, Dictionary<string, string> additionalHeaders = null, bool isIgnoreRefreshToken = false)
        {
            additionalHeaders = GenerateAuthorizationHeader(additionalHeaders);

            using (var response = await MakeRequestAsync(method, requestUrl, requestBody, additionalHeaders, isIgnoreRefreshToken).ConfigureAwait(false))
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

        private string RenameDataProperty<T>(string data)
        {
            var propertyName = GetPropertyName<T>();
            int indexProperty = data.IndexOf(propertyName);
            data = data.Remove(indexProperty, propertyName.Length).Insert(indexProperty, nameof(GenericGetResult<T>.Result));

            return data;
        }

        private string GetPropertyName<T>()
        {
            var propertyKey = typeof(T).FullName;

            foreach (var property in _propertyNames)
            {
                if (propertyKey.Contains(property.Key))
                {
                    return _propertyNames[property.Key];
                }
            }

            return string.Empty;
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(HttpMethod method, string requestUrl, object requestBody = null, Dictionary<string, string> additioalHeaders = null, bool isIgnoreRefreshToken = false)
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

            if (_settingsManager.IsAuthorizationComplete && !isIgnoreRefreshToken)
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
                var resultData = await RequestAsync<RefreshTokenQueryResultExecutionResult>(HttpMethod.Post, $"{Constants.API.HOST_URL}/api/auth/refresh-token", responseBody, null, true);

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

        private Dictionary<string, string> GenerateAuthorizationHeader(Dictionary<string, string> additionalHeaders)
        {
            if (additionalHeaders is null)
            {
                additionalHeaders = new();
            }

            additionalHeaders.Add("Authorization", $"Bearer {_settingsManager.Token}");

            return additionalHeaders;
        }

        #endregion
    }
}
