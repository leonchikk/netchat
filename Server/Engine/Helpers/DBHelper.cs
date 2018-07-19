using NetLibrary.Models;
using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Text;

namespace Server.Engine.Helpers
{
    public static class DBHelper
    {
        public static bool TryAuthorizationUser(JObject authData, out CommandModel result)
        {
            try
            {
                string email = authData["Email"].ToString();
                string password = authData["Password"].ToString();

                using (var dbContext = new MessangerContext())
                {
                    if(string.IsNullOrEmpty(email))
                    {
                        result = CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { { "Message", "Email could not be empty!" } });
                        return false;
                    }

                    var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                    if (user == null)
                    {
                        result = CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { {"Message", "There is no user with such email or password"} });
                        return false;
                    }

                    // generate and send back user auth token
                    var plainTextBytes = Encoding.UTF8.GetBytes($"{user.Email}:{user.Password}");
                    var token = Convert.ToBase64String(plainTextBytes);

                    result = CommandHelper.GetCommandResultData(StatusCodes.OK, new JObject { { "Message", "OK" }, { "Token", token} });

                    return true;
                }
            }
            catch (Exception ex)
            {
                result = CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { { "Message", ex.Message } });
                return false;
            }
            
        }

        public static bool TryRegisterUser(JObject registerData, out CommandModel result)
        {
            try
            {
                string name = registerData["Name"].ToString();
                string email = registerData["Email"].ToString();
                string password = registerData["Password"].ToString();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                {
                    result = CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { { "Message", "Not all required fields are filled" } });
                    return false;
                }

                using (var dbContext = new MessangerContext())
                {
                    if(dbContext.Users.FirstOrDefault(u => u.Email == email) != null)
                    {
                        result = CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { { "Message", "There is user with same email exist!" } });
                        return false;
                    }

                    dbContext.Users.Add(new User {  Id = Guid.NewGuid(), Email = email, Name = name, Password = password });
                    dbContext.SaveChanges();
                }

                result = CommandHelper.GetCommandResultData(StatusCodes.OK, new JObject { { "Message", "OK" } });

                return true;
            }
            catch (Exception ex)
            {
                result = CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { { "Message", ex.Message } });

                return false;
            }
        }

        public static CommandModel GetUserInfo(JObject param)
        {
            var token = param["Token"].ToString();

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerContext())
            {

                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return CommandHelper.GetCommandResultData(StatusCodes.BadRequest, new JObject { { "Message", "There is user does not exist!" } });


                return CommandHelper.GetCommandResultData(StatusCodes.OK, new JObject { { "UserId", user.Id }, { "UserName", user.Name }, { "UserEmail", user.Email } });
            }
        }
    }
}
