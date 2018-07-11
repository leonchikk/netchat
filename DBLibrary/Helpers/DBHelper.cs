using NetLibrary.Models;
using Newtonsoft.Json.Linq;
using System;

namespace DBLibrary.Helpers
{
    public static class DBHelper
    {
        public static bool TryAuthorizationUser(JObject authData, out CommandModel result)
        {
            result = null;

            return false;
        }

        public static bool TryRegisterUser(JObject authData, out CommandModel result)
        {
            result = null;

            return false;
        }
    }
}
