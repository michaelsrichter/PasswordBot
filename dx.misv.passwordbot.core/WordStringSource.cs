using System;
using dx.misv.passwordbot.core.Properties;

namespace dx.misv.passwordbot.core
{
    public class WordStringSource : IStringSource
    {
        private readonly Random _rand = new Random();
        public string Get(PasswordRequestOptions options)
        {
            var adjectives = Resources.adjectives.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var nouns = Resources.nouns.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return $"{adjectives[_rand.Next(0, adjectives.Length - 1)]} {nouns[_rand.Next(0, nouns.Length - 1)]}";
        }
    }
}