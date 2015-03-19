using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnyDoDotNet
{
    public sealed class Util
    {
        public static string CreateGlobalId()
        {
            var randomString = "";
            Random rand = new Random();

            for (var i = 0; i < 16; i++)
            {
                randomString += (char)(rand.NextDouble() * 256);
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(randomString)).Replace("/", "_").Replace("+", "-");
        }
    }
}
