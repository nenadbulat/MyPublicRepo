using System.Net;
using UsersManipulationWeb.Interfaces;
using UsersManipulationWeb.Util;

namespace UsersManipulationWeb.ApiKey
{
    public class ApiKeyValidation : IApiKeyValidation
    {
        private readonly IConfiguration configuration;

        public ApiKeyValidation(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool IsValidApiKey(string userApiKey)
        {
            if (string.IsNullOrEmpty(userApiKey))
            {
                return false;
            }

            var apiKey = configuration.GetValue<string>(Constants.ApiKeyName);
            if (apiKey is null || !apiKey.Equals(userApiKey))
            {
                return false;
            }

            return true;
        }
    }
}
