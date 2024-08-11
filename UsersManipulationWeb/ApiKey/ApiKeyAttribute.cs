using Microsoft.AspNetCore.Mvc;

namespace UsersManipulationWeb.ApiKey
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute(): base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
