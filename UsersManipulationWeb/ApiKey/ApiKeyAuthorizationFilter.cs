using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UsersManipulationWeb.Interfaces;
using UsersManipulationWeb.Util;

namespace UsersManipulationWeb.ApiKey
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IApiKeyValidation apiKeyValidation;

        public ApiKeyAuthorizationFilter(IApiKeyValidation apiKeyValidation)
        {
            this.apiKeyValidation = apiKeyValidation;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userApiKey = context.HttpContext.Request.Headers[Constants.ApiKeyHeaderName];

            if (string.IsNullOrEmpty(userApiKey))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!apiKeyValidation.IsValidApiKey(userApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
