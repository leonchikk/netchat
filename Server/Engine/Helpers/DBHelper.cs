using NetLibrary.Models;
using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace Server.Engine.Helpers
{
    public static class DBHelper
    {
        public static bool TryAuthorizationUser(JObject authData, out CommandModel result)
        {
            try
            {
                string login = authData["Login"].ToString();
                string password = authData["Password"].ToString();

                using (var dbContext = new MessangerContext())
                {
                    var user = dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                    if (user == null)
                    {
                        result = new CommandModel
                        {
                            StatusCode = StatusCodes.BadRequest,

                            Data = new JObject
                            {
                                {"Message", "There is no user with such email or password"}

                            }.ToString()
                        };
                        return false;
                    }
                }
                result = new CommandModel
                {
                    StatusCode = StatusCodes.OK,
                    Data = new JObject
                    {
                        {"Message","OK"}

                    }.ToString()
                };

                return true;
            }
            catch (Exception ex)
            {
                result = new CommandModel
                {
                    StatusCode = StatusCodes.BadRequest,

                    Data = new JObject
                    {
                        {"Message", ex.Message}
                    }.ToString()
                };

                return false;
            }
            
        }

        public static bool TryRegisterUser(JObject registerData, out CommandModel result)
        {
            try
            {
                string login = registerData["Login"].ToString();
                string name = registerData["Name"].ToString();
                string password = registerData["Password"].ToString();

                using (var dbContext = new MessangerContext())
                {
                    dbContext.Users.Add(new User {  Id = Guid.NewGuid(), Login = login, Name = name, Password = password });
                    dbContext.SaveChanges();
                }

                result = new CommandModel
                {
                    StatusCode = StatusCodes.OK,
                    Data = new JObject
                    {
                        {"Message","OK"}

                    }.ToString()
                };

                return true;
            }
            catch (Exception ex)
            {
                result = new CommandModel
                {
                    StatusCode = StatusCodes.BadRequest,

                    Data = new JObject
                    {
                        {"Message", ex.Message}

                    }.ToString()
                };

                return false;
            }
        }
    }
}
