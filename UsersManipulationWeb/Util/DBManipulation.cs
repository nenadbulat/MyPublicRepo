using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Net;
using System.Security.Cryptography;
using UsersManipulationWeb.Model;

namespace UsersManipulationWeb.Util
{
    public class DBManipulation
    {
        private MySqlConnection dbConnection;
        private MySqlCommand sqlCommand;

        public DBManipulation()
        {
            Initialize();
        }

        private void Initialize()
        {
            string connectionString = string.Format("SERVER={0};PORT={1};DATABASE={2};UID={3};PASSWORD={4};",
                Resources.SERVER,
                Resources.PORT,
                Resources.DATABASE,
                Resources.UID,
                Resources.PASSWORD);
            dbConnection = new MySqlConnection(connectionString);
        }

        private bool OpenConnectionToDB()
        {
            try
            {
                dbConnection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during opening connection to DB. Stack trace: {0}. Error message: {1}", ex.StackTrace, ex.Message));
                return false;
            }
        }

        private bool CloseConnectionToDB()
        {
            try
            {
                dbConnection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during closing connection to DB. Stack trace: {0}. Error message: {1}", ex.StackTrace, ex.Message));
                return false;
            }
        }

        public User GetUser(string username)
        {
            User user = new User();
            string query = string.Format("SELECT * FROM user WHERE username = '{0}'", username);
            MySqlDataReader dataReader = null;
            if (this.OpenConnectionToDB() == true)
            {
                sqlCommand = new MySqlCommand(query, dbConnection);
                try
                {
                    dataReader = sqlCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        user.Id = int.Parse(dataReader["id"].ToString());
                        user.UserName = dataReader["username"].ToString();
                        user.FullName = dataReader["fullname"].ToString();
                        user.EMail = dataReader["email"].ToString();
                        user.MobileNumber = dataReader["mobilenumber"].ToString();
                        user.Language = (Enums.Language)int.Parse(dataReader["language"].ToString());
                        user.Culture = (Enums.Culture)int.Parse(dataReader["culture"].ToString());
                        user.Password = dataReader["password"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during reading data from DB. Stack trace: {0}. Error message: {1}", ex.StackTrace, ex.Message));
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    this.CloseConnectionToDB();
                }
            }

            return user;
        }

        public bool AddUser(User user)
        {
            int retVal = 0;
            if (!CheckUsernameExistance(user.UserName))
            {
                if (this.OpenConnectionToDB() == true)
                {
                    try
                    {
                        string insertCommand = string.Format(
                            "INSERT INTO user (username, fullname, email, mobilenumber, language, culture, password) VALUES ('{0}', '{1}', '{2}', '{3}', {4}, {5}, '{6}')",
                            user.UserName,
                            user.FullName,
                            user.EMail,
                            user.MobileNumber,
                            (int)user.Language,
                            (int)user.Culture,
                            user.Password
                        );

                        sqlCommand = new MySqlCommand(insertCommand, dbConnection);
                        retVal = sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error adding user with username: {0} to DB. Stack trace: {1}. Error message: {2}", 
                            user.UserName, 
                            ex.StackTrace, 
                            ex.Message)
                        );
                    }
                    finally
                    {
                        this.CloseConnectionToDB();
                    }
                }
                else
                {
                    Logger.LogMessage(Enums.LogLevel.Error, "Can't add user. Connection to DB is not open.");
                }
            }
            else
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("Can't add user. User with username: {0} already exist.", user.UserName));
            }

            return retVal > 0;
        }

        public bool UpdateUser(string username, User user)
        {
            int retVal = 0;
            if (this.OpenConnectionToDB() == true)
            {
                try
                {
                    string updateCommand = string.Format(
                        "UPDATE user SET fullname = '{0}', email = '{1}', mobilenumber = '{2}', language = {3}, culture = {4}, password = '{5}', username = '{6}' WHERE username = '{7}'",
                        user.FullName,
                        user.EMail,
                        user.MobileNumber,
                        (int)user.Language,
                        (int)user.Culture,
                        user.Password,
                        user.UserName,
                        username
                    );

                    sqlCommand = new MySqlCommand(updateCommand, dbConnection);
                    retVal = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error updating user with username: {0} in DB. Stack trace: {1}. Error message: {2}", 
                        username, 
                        ex.StackTrace, 
                        ex.Message)
                    );
                }
                finally
                {
                    this.CloseConnectionToDB();
                }
            }

            if (retVal <= 0)
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("User with username: {0} does not exist.", username));
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool DeleteUser(string username)
        {
            int retVal = 0;
            if (this.OpenConnectionToDB() == true)
            {
                try
                {
                    string deleteCommand = string.Format("DELETE FROM user WHERE username = '{0}'", username);
                    sqlCommand = new MySqlCommand(deleteCommand, dbConnection);
                    retVal = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error deleting user with username: {0} from DB. Stack trace: {1}. Error message: {2}",
                        username,
                        ex.StackTrace,
                        ex.Message)
                    );
                }
                finally
                {
                    this.CloseConnectionToDB();
                }
            }

            if (retVal <= 0)
            {
                Logger.LogMessage(Enums.LogLevel.Error, string.Format("User with username: {0} does not exist.", username));
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckUsernameExistance(string username)
        {
            string query = string.Format("SELECT Count(*) FROM user WHERE username = '{0}'", username);
            int count = 0;
            if (this.OpenConnectionToDB() == true)
            {
                try
                {
                    sqlCommand = new MySqlCommand(query, dbConnection);
                    count = int.Parse(sqlCommand.ExecuteScalar().ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during checking user with username: {0} existance in DB. Stack trace: {1}. Error message: {2}",
                        username,
                        ex.StackTrace,
                        ex.Message)
                    );
                }
                finally
                {
                    this.CloseConnectionToDB();
                }
            }

            return count > 0;
        }

        public bool ValidatePassword(string username, string passwordToCheck)
        {
            string query = string.Format("SELECT password FROM user WHERE username = '{0}'", username);
            string password = string.Empty;
            MySqlDataReader dataReader = null;
            if (this.OpenConnectionToDB() == true)
            {
                try
                {
                    sqlCommand = new MySqlCommand(query, dbConnection);
                    dataReader = sqlCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        password = dataReader["password"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during password validation for user with username: {0}. Stack trace: {1}. Error message: {2}",
                        username,
                        ex.StackTrace,
                        ex.Message)
                    );
                }
                finally
                {
                    if (dataReader != null)
                    {
                       dataReader.Close();
                    }

                    this.CloseConnectionToDB();
                }
            }

            if (password.Equals(passwordToCheck))
            {
                return true;
            }
            else
            {
                Logger.LogMessage(Enums.LogLevel.Info, string.Format("Password provided for user with username: {0} is NOT valid.", username));
                return false;
            }
        }

        public string GetAPIKey(string client)
        {
            string query = string.Format("SELECT apikey FROM apikeys WHERE id = '{0}'", client);
            string apiKey = string.Empty;
            MySqlDataReader dataReader = null;
            if (this.OpenConnectionToDB() == true)
            {
                try
                {
                    sqlCommand = new MySqlCommand(query, dbConnection);
                    dataReader = sqlCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        apiKey = dataReader["apikey"].ToString();
                    }

                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        Logger.LogMessage(Enums.LogLevel.Info, "API key found in DB.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during getting API key. Stack trace: {0}. Error message: {1}",
                        ex.StackTrace,
                        ex.Message)
                    );
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    this.CloseConnectionToDB();
                }
            }

            return apiKey;
        }

        public bool RegisterAPIKey(string client, out string apiKey)
        {
            int retVal = 0;
            apiKey = GetAPIKey(client);
            if (apiKey.Equals(string.Empty))
            {
                if (this.OpenConnectionToDB() == true)
                {
                    try
                    {
                        using (var cryptoProvider = new RNGCryptoServiceProvider())
                        {
                            byte[] bytes = new byte[64];
                            cryptoProvider.GetBytes(bytes);
                            apiKey = Convert.ToBase64String(bytes);
                        }

                        string insertCommand = string.Format("INSERT INTO apikeys (id, apikey) VALUES ('{0}', '{1}')", client, apiKey);
                        sqlCommand = new MySqlCommand(insertCommand, dbConnection);
                        retVal = sqlCommand.ExecuteNonQuery();
                        if (retVal > 0)
                        {
                            Logger.LogMessage(Enums.LogLevel.Info, "API key registered.");
                            return true;
                        }
                        else
                        {
                            Logger.LogMessage(Enums.LogLevel.Info, "API key not registered.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogMessage(Enums.LogLevel.Error, string.Format("Error during API key registration. Stack trace: {0}. Error message: {1}",
                            ex.StackTrace,
                            ex.Message)
                        );
                    }
                    finally
                    {
                        this.CloseConnectionToDB();
                    }
                }
            }

            return retVal > 0;
        }
    }
}
