using System;
using System.Linq;

namespace dx.misv.passwordbot.core
{
    public class RandomStringSource : IStringSource
    {
        private readonly string  _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _rand = new Random();
        public string Get(PasswordRequestOptions options)
        {
            return new string(Enumerable.Repeat(_chars, options.MinimumLength)
                .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }
    }
}