using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Net;
using System.Net;
using System.Security.Cryptography;
using UsersManipulationWeb.ApiKey;
using UsersManipulationWeb.Model;
using UsersManipulationWeb.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UsersManipulationWeb.Controllers
{
    [Route("[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DBManipulation dbManipulation = new DBManipulation();

        [HttpGet]
        public string RegisterAPIKey()
        {
            string address = HttpContext.Connection.RemoteIpAddress.ToString();
            string hostName = Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName;
            string apiKey = string.Empty;

            Logger.LogMethodCall(address, hostName, nameof(this.RegisterAPIKey), "-");

            if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(hostName))
            {
                dbManipulation.RegisterAPIKey(address + "_" + hostName, out apiKey);
            }

            return apiKey;
        }

        [HttpGet]
        [ApiKey]
        public bool ValidatePassword(string username, string passwordToCheck)
        {
            Logger.LogMethodCall(
                HttpContext.Connection.RemoteIpAddress.ToString(),
                Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName,
                nameof(this.ValidatePassword),
                nameof(username) + ": " + username + "\n\t" + nameof(passwordToCheck) + ": " + passwordToCheck
            );

            if (string.IsNullOrEmpty(username))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid parameter: {0}.", nameof(username)));
                return false;
            }

            if (string.IsNullOrEmpty(passwordToCheck))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid parameter: {0}.", nameof(passwordToCheck)));
                return false;
            }

            if (dbManipulation.ValidatePassword(username, passwordToCheck))
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("Password for user with username: {0} is valid.", username));
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        [Produces("application/json")]
        [ApiKey]
        public User GetUser(string username)
        {
            Logger.LogMethodCall(
                HttpContext.Connection.RemoteIpAddress.ToString(),
                Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName,
                nameof(this.GetUser),
                nameof(username) + ": " + username
            );

            if (string.IsNullOrEmpty(username))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid parameter: {0}.", nameof(username)));
                return null;
            }

            User user = dbManipulation.GetUser(username);
            if (!string.IsNullOrEmpty(user.UserName))
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("User with username: {0} found.", username));
                return user;
            }
            else
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("User with username: {0} not found.", username));
                return null;
            }
        } 


        [HttpPost]
        [ApiKey]
        public bool AddUser([FromBody] User user)
        {
            Logger.LogMethodCall(
                HttpContext.Connection.RemoteIpAddress.ToString(),
                Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName,
                nameof(this.AddUser),
                nameof(user) + ": " + JsonConvert.SerializeObject(user)
            );

            if (user == null)
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Parameter: {0} is null.", nameof(user)));
                return false;
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.UserName)));
                return false;
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.Password)));
                return false;
            }

            if (!Enum.IsDefined(typeof(Enums.Language), user.Language))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.Language)));
                return false;
            }

            if (!Enum.IsDefined(typeof(Enums.Culture), user.Culture))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.Culture)));
                return false;
            }

            if (dbManipulation.AddUser(user))
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("User with username: {0} added.", user.UserName));
                return true;
            }
            else
            {
                return false;
            }

        }

        [HttpPut]
        [ApiKey]
        public bool UpdateUser(string username, [FromBody] User user)
        {
            Logger.LogMethodCall(
                HttpContext.Connection.RemoteIpAddress.ToString(),
                Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName,
                nameof(this.UpdateUser),
                nameof(username) + ": " + username + "\n\t" + nameof(user) + ": " + JsonConvert.SerializeObject(user)
            );

            if (user == null)
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Parameter: {0} is null.", nameof(user)));
                return false;
            }

            if (string.IsNullOrEmpty(username))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid parameter: {0}.", nameof(username)));
                return false;
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.UserName)));
                return false;
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.Password)));
                return false;
            }

            if (!Enum.IsDefined(typeof(Enums.Language), user.Language))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.Language)));
                return false;
            }

            if (!Enum.IsDefined(typeof(Enums.Culture), user.Culture))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid property: {0}.", nameof(user.Culture)));
                return false;
            }

            if (dbManipulation.UpdateUser(username, user))
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("User with username: {0} updated.", username));
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpDelete]
        [ApiKey]
        public bool DeleteUser(string username)
        {
            Logger.LogMethodCall(
                HttpContext.Connection.RemoteIpAddress.ToString(),
                Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName,
                nameof(this.DeleteUser),
                nameof(username) + ": " + username
            );

            if (string.IsNullOrEmpty(username))
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Invalid parameter: {0}.", nameof(username)));
                return false;
            }

            if (dbManipulation.DeleteUser(username))
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("User with username: {0} deleted.", username));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
