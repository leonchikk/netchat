using NetLibrary.Models;
using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

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
                        result = CommandHelper.GetCommandResultData(CommandTypes.Authorization, StatusCodes.BadRequest, new JObject { { "Message", "Email could not be empty!" } });
                        return false;
                    }

                    var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                    if (user == null)
                    {
                        result = CommandHelper.GetCommandResultData(CommandTypes.Authorization, StatusCodes.BadRequest, new JObject { {"Message", "There is no user with such email or password"} });
                        return false;
                    }

                    // generate and send back user auth token
                    var plainTextBytes = Encoding.UTF8.GetBytes($"{user.Email}:{user.Password}");
                    var token = Convert.ToBase64String(plainTextBytes);

                    result = CommandHelper.GetCommandResultData(CommandTypes.Authorization, StatusCodes.OK, new JObject { { "Message", "OK" }, { "Token", token} });

                    return true;
                }
            }
            catch (Exception ex)
            {
                result = CommandHelper.GetCommandResultData(CommandTypes.Authorization, StatusCodes.BadRequest, new JObject { { "Message", ex.Message } });
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
                    result = CommandHelper.GetCommandResultData(CommandTypes.Registration, StatusCodes.BadRequest, new JObject { { "Message", "Not all required fields are filled" } });
                    return false;
                }

                using (var dbContext = new MessangerContext())
                {
                    if(dbContext.Users.FirstOrDefault(u => u.Email == email) != null)
                    {
                        result = CommandHelper.GetCommandResultData(CommandTypes.Registration, StatusCodes.BadRequest, new JObject { { "Message", "There is user with same email exist!" } });
                        return false;
                    }

                    dbContext.Users.Add(new User {  Id = Guid.NewGuid(), Email = email, Name = name, Password = password });
                    dbContext.SaveChanges();
                }

                result = CommandHelper.GetCommandResultData(CommandTypes.Registration, StatusCodes.OK, new JObject { { "Message", "OK" } });

                return true;
            }
            catch (Exception ex)
            {
                result = CommandHelper.GetCommandResultData(CommandTypes.Registration, StatusCodes.BadRequest, new JObject { { "Message", ex.Message } });

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
                    return CommandHelper.GetCommandResultData(CommandTypes.UserInfo, StatusCodes.BadRequest, new JObject { { "Message", "There is user does not exist!" } });


                return CommandHelper.GetCommandResultData(CommandTypes.UserInfo, StatusCodes.OK, new JObject { { "UserId", user.Id }, { "UserName", user.Name }, { "UserEmail", user.Email } });
            }
        }

        public static CommandModel GetSearchResults(JObject param)
        {
            var token = param["Token"].ToString();
            var filter = param["Filter"].ToString();

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerContext())
            {

                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return CommandHelper.GetCommandResultData(CommandTypes.Search, StatusCodes.BadRequest, new JObject { { "Message", "There is user does not exist!" } });
                

                var filteredUsers = dbContext.Users.Where(x => x.Id != user.Id &&
                                                            (x.Name.Contains(filter) || x.Email.Contains(filter)))
                                                 .OrderBy(x => x.Name)
                                                 .Take(500)
                                                 .ToList();

                var searchItems = new List<SearchItem>();
                searchItems.AddRange(filteredUsers.Select(u => SearchItem.Copy(u, user.Id)));

                var result = JArray.Parse(JsonConvert.SerializeObject(searchItems));
                

                return CommandHelper.GetCommandResultData(CommandTypes.Search, StatusCodes.OK, result);
            }
        }

       
        public static bool IsUserFriend(User compared, Guid currentUserId)
        {
            using (var dbContext = new MessangerContext())
            {
                if (dbContext.Contacts.Any(c => (c.FirstUser.Id == currentUserId && c.SecondUser.Id == compared.Id) ||
                                                (c.FirstUser.Id == compared.Id && c.SecondUser.Id == currentUserId) ))
                    return true;
            }

            return false;
        }
    }

    #region Helper class
    public class SearchItem
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsFriend { get; set; }

        public static SearchItem Copy(User user, Guid currentUserId)
        {
            return new SearchItem
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsFriend = DBHelper.IsUserFriend(user, currentUserId)
            };
        }
        
    }
    #endregion
}
