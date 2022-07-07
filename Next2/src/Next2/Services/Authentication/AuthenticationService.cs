using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.Commands;
using Next2.Models.API.Queries;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Rest;
using Next2.Services.Settings;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRestService _restService;
        private readonly ISettingsManager _settingsManager;

        public AuthenticationService(
            ISettingsManager settingsManager,
            IRestService restService)
        {
            _restService = restService;
            _settingsManager = settingsManager;
        }

        #region -- Public properties --

        public int AuthorizedUserId => _settingsManager.UserId;

        public bool IsAuthorizationComplete => _settingsManager.IsAuthorizationComplete;

        public bool IsUserAdmin => _settingsManager.IsUserAdmin;

        #endregion

        #region -- IAuthenticationService implementation --

        public async Task<AOResult<LoginQueryResult>> GetUserById(string userId)
        {
            var result = new AOResult<LoginQueryResult>();

            var employee = new LoginQuery()
            {
                EmployeeId = userId,
            };

            try
            {
                if (int.TryParse(userId, out int id))
                {
                    var query = $"{Constants.API.HOST_URL}/api/auth/login";

                    var response = await _restService.RequestAsync<GenericExecutionResult<LoginQueryResult>>(HttpMethod.Post, query, employee);

                    if (response.Success && response.Value is not null)
                    {
                        result.SetSuccess(response.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetUserById)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AuthorizeUserAsync(string userId)
        {
            var result = new AOResult();

            var employee = new LoginQuery()
            {
                EmployeeId = userId,
            };

            try
            {
                if (int.TryParse(userId, out int id))
                {
                    var query = $"{Constants.API.HOST_URL}/api/auth/login";

                    var response = await _restService.RequestAsync<GenericExecutionResult<LoginQueryResult>>(HttpMethod.Post, query, employee);

                    if (response.Success)
                    {
                        _settingsManager.UserId = id;
                        _settingsManager.IsAuthorizationComplete = true;
                        _settingsManager.Token = response.Value.Tokens.AccessToken;
                        _settingsManager.RefreshToken = response.Value.Tokens.RefreshToken;
                        _settingsManager.TokenExpirationDate = DateTime.Now.AddHours(Constants.API.TOKEN_EXPIRATION_TIME);

                        var roles = response.Value.Roles;

                        if (roles is not null)
                        {
                            _settingsManager.IsUserAdmin = roles.Contains(Constants.ROLE_ADMIN);
                        }

                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AuthorizeUserAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> LogoutAsync()
        {
            var result = new AOResult();

            var employee = new LogoutCommand()
            {
                EmployeeId = _settingsManager.UserId.ToString(),
                RefreshToken = _settingsManager.RefreshToken,
            };

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/auth/logout";

                var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Post, query, employee);

                if (response.Success)
                {
                    _settingsManager.Clear();

                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(LogoutAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public void ClearSession()
        {
            _settingsManager.Clear();
        }

        #endregion
    }
}
