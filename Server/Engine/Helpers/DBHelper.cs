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

                using (var dbContext = new MessangerModel())
                {
                    if(string.IsNullOrEmpty(email))
                    {
                        result = CommandHelper.GetCommandResultData(CommandTypes.Authorization, StatusCodes.BadRequest, new JObject {
                            { "Message", "Email could not be empty!" }
                        });

                        return false;
                    }

                    var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                    if (user == null)
                    {
                        result = CommandHelper.GetCommandResultData(CommandTypes.Authorization, StatusCodes.BadRequest, new JObject {
                            { "Message", "There is no user with such email or password"}
                        });

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
                    result = CommandHelper.GetCommandResultData(CommandTypes.Registration, StatusCodes.BadRequest, new JObject {
                        { "Message", "Not all required fields are filled" }
                    });

                    return false;
                }

                using (var dbContext = new MessangerModel())
                {
                    if(dbContext.Users.FirstOrDefault(u => u.Email == email) != null)
                    {
                        result = CommandHelper.GetCommandResultData(CommandTypes.Registration, StatusCodes.BadRequest, new JObject {
                            { "Message", "There is user with same email exist!" }
                        });

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

        public static Tuple<CommandModel, Guid> GetUserInfo(JObject param)
        {
            var token = param["Token"].ToString();

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerModel())
            {

                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.UserInfo, StatusCodes.BadRequest, new JObject {
                        { "Message", "There is user does not exist!" }
                    }), Guid.Empty);


                return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.UserInfo, StatusCodes.OK, new JObject {
                    { "UserId", user.Id },
                    { "UserName", user.Name },
                    { "UserEmail", user.Email }
                }), user.Id);
            }
        }

        public static CommandModel GetContacts(JObject param)
        {
            var token = param["Token"].ToString();
            var userId = Guid.Parse(param["UserId"].ToString());

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerModel())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return CommandHelper.GetCommandResultData(CommandTypes.GetContacts, StatusCodes.BadRequest, new JObject { { "Message", "There is user does not exist!" } });

                var userContacts = dbContext.Contacts.Where(contact => contact.User1 == userId || contact.User2 == userId )
                                                   .Take(500)
                                                   .ToList();

                var contacts = new List<ContactItem>();
                contacts.AddRange(userContacts.Select(c => ContactItem.Copy(c, user.Id)));

                var result = JArray.Parse(JsonConvert.SerializeObject(contacts));

                return CommandHelper.GetCommandResultData(CommandTypes.GetContacts, StatusCodes.OK, result);
            }
        }

        public static Tuple<CommandModel, Guid> RemoveContact(JObject param)
        {
            var token = param["Token"].ToString();
            var initiatorId = Guid.Parse(param["InitiatorId"].ToString());
            var targetId = Guid.Parse(param["TargetId"].ToString());

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerModel())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.RemoveContact, StatusCodes.BadRequest, new JObject {
                        { "Message", "There is user does not exist!" }
                    }), Guid.Empty);

                var targetUser = dbContext.Contacts.FirstOrDefault(c => (c.User1 == initiatorId && c.User2 == targetId)
                                                                              || (c.User2 == initiatorId && c.User1 == targetId));

                dbContext.Contacts.Remove(targetUser);
                dbContext.SaveChanges();

                JObject response = new JObject
                {
                    {"TargetId",  targetId}
                };

                return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.RemoveContact, StatusCodes.OK, response), targetId);
            }
        }

        public static Tuple<CommandModel, Guid> AddNewContact(JObject param)
        {
            var token = param["Token"].ToString();
            var initiatorId = Guid.Parse(param["InitiatorId"].ToString());
            var targetId = Guid.Parse(param["TargetId"].ToString());

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerModel())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.SendToApproveContact, StatusCodes.BadRequest, new JObject {
                        { "Message", "There is user does not exist!" }
                    }), Guid.Empty);

                var newContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    User1 = initiatorId,
                    User2 = targetId,
                    IsApproved = false
                };

                dbContext.Contacts.Add(newContact);
                dbContext.SaveChanges();

                JObject response = new JObject
                {
                    {"TargetId",  targetId}
                };

                return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.SendToApproveContact, StatusCodes.OK, response), targetId);
            }
        }

        public static Tuple<CommandModel, Guid> ApproveContact(JObject param)
        {
            var token = param["Token"].ToString();
            var initiatorId = Guid.Parse(param["InitiatorId"].ToString());
            var targetId = Guid.Parse(param["TargetId"].ToString());

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerModel())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                    return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.ApproveContact, StatusCodes.BadRequest, new JObject {
                        { "Message", "There is user does not exist!" }
                    }), Guid.Empty);

                var contact = dbContext.Contacts.FirstOrDefault(c => (c.User1 == initiatorId && c.User2 == targetId) ||
                                                                     (c.User2 == initiatorId && c.User1 == targetId));
                if (contact == null)
                    return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.ApproveContact, StatusCodes.BadRequest, new JObject {
                        { "Message", "There is contact does not exist!" }
                    }), Guid.Empty);

                contact.IsApproved = true;
                dbContext.SaveChanges();

                JObject response = new JObject
                {
                    {"TargetId",  targetId}
                };

                return Tuple.Create(CommandHelper.GetCommandResultData(CommandTypes.ApproveContact, StatusCodes.OK, response), targetId);
            }
        }

        #region Helper methods
        public static CommandModel GetSearchResults(JObject param)
        {
            var token = param["Token"].ToString();
            var filter = param["Filter"].ToString();

            string authData = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenParts = authData.Split(':');
            var email = tokenParts[0];
            var password = tokenParts[1];

            using (var dbContext = new MessangerModel())
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

        public static User GetUserFromContact(Contact compared, Guid currentUserId)
        {
            using (var dbContext = new MessangerModel())
            {
                var userFromContact = dbContext.Users.FirstOrDefault(user => (user.Id == compared.User1 || user.Id == compared.User2) && user.Id != currentUserId);
                return userFromContact;
            }
        }

        public static bool IsSendedToApprove(User compared, Guid currentUserId)
        {
            using (var dbContext = new MessangerModel())
            {
                var contactUser = dbContext.Contacts.FirstOrDefault(cu => cu.User1 == compared.Id && cu.User2 == currentUserId);

                if (contactUser != null)
                    return true;
                
                return false;
            }
        }

        public static bool IsContactApproved(User compared, Guid currentUserId)
        {
            using (var dbContext = new MessangerModel())
            {
                if (dbContext.Contacts.Any(c => ((c.FirstUser.Id == currentUserId && c.SecondUser.Id == compared.Id) ||
                                                (c.FirstUser.Id == compared.Id && c.SecondUser.Id == currentUserId)) && c.IsApproved))
                    return true;
            }

            return false;
        }

        public static bool IsUserFriend(User compared, Guid currentUserId)
        {
            using (var dbContext = new MessangerModel())
            {
                if (dbContext.Contacts.Any(c => (c.FirstUser.Id == currentUserId && c.SecondUser.Id == compared.Id) ||
                                                (c.FirstUser.Id == compared.Id && c.SecondUser.Id == currentUserId)))
                    return true;
            }

            return false;
        }
        #endregion
    }

    #region Helper class
    public class SearchItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsFriend { get; set; }
        public bool IsApproved { get; set; }
        public bool IsInitiatorToApprove { get; set; }

        public static SearchItem Copy(User user, Guid currentUserId)
        {
            return new SearchItem
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsFriend = DBHelper.IsUserFriend(user, currentUserId),
                IsApproved = DBHelper.IsContactApproved(user, currentUserId),
                IsInitiatorToApprove = DBHelper.IsSendedToApprove(user, currentUserId)
            };
        }
        
    }

    public class ContactItem
    {
        //public Guid Id { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsFriend { get; set; }
        public bool IsApproved { get; set; }
        public bool IsInitiatorToApprove { get; set; }

        public static ContactItem Copy(Contact contact, Guid currentUserId)
        {
            var userContact = DBHelper.GetUserFromContact(contact, currentUserId);

            return new ContactItem
            {
                //Id = contact.Id,
                Id = userContact.Id,
                Name = userContact.Name,
                Email = userContact.Email,
                IsFriend = true,
                IsApproved = contact.IsApproved,
                IsInitiatorToApprove = DBHelper.IsSendedToApprove(userContact, currentUserId)
            };
        }

    }
    #endregion
}
