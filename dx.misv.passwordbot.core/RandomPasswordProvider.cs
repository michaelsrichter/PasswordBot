using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace dx.misv.passwordbot.core
{
    public class RandomPasswordProvider : IPasswordProvider
    {
        private readonly IStringSource _stringSource;

        public RandomPasswordProvider(IStringSource stringSource)
        {
            _stringSource = stringSource;
        }

        private readonly Random _rand = new Random();

        public string GetPassword(PasswordRequestOptions options)
        {
            return PasswordGenerator(options);
        }

        internal string PasswordGenerator(PasswordRequestOptions options)
        {           
            var goodPassword = false;

            var password = string.Empty;
            while (!goodPassword)
            {
                password = _stringSource.Get(options);

                if (options.CaseType.GetType() == typeof(CaseTypeLettersAndNumbersMixedCasing) ||
                    options.CaseType.GetType() == typeof(CaseTypeLettersMixedCasing))
                {
                    password = GenerateMixedCase(password);
                }

                if (options.CaseType.GetType() == typeof (CaseTypeNumbers) ||
                    options.CaseType.GetType() == typeof (CaseTypeLettersAndNumbers) ||
                    options.CaseType.GetType() == typeof (CaseTypeLettersAndNumbersMixedCasing))
                {
                    password = GenerateNumbers(password);
                }

                if (options.SpecialCharacter)
                {
                    password = GenerateSpecialCharacter(password);
                }

                password = FinalizePassword(password);
                goodPassword = IsGoodPassword(password, options);
            }


            return password;
        }

        internal string FinalizePassword(string password)
        {
            //just remove any spaces
            return Regex.Replace(password, @"\s+", "");
        }

        internal string GenerateNumbers(string password)
        {
            if (password.Any(char.IsNumber))
                return password;

            var isLetter = false;
            var index = 0;
            while (!isLetter)
            {
                index = _rand.Next(0, password.Length - 1);
                isLetter = char.IsLetter(password[index]);
            }
            var val = _rand.Next(0, 9);
            switch (password[index].ToString().ToLower())
            {
                case "o":
                    val = 0;
                    break;
                case "l":
                    val = 1;
                    break;
                case "e":
                    val = 3;
                    break;
                case "s":
                    val = 5;
                    break;
                case "t":
                    val = 7;
                    break;
                case "b":
                    val = 6;
                    break;
                default:
                    break;
            }

            password = password.Remove(index, 1);
            password = password.Insert(index, val.ToString());
            return password;
        }

        internal static bool IsGoodPassword(string password, PasswordRequestOptions options)
        {
            var lengthCheck = password.Length >= options.MinimumLength;
            if (!lengthCheck) return false;
            var passCheck = options.CaseType.Check(password);
            if (!passCheck) return false;

            if (options.SpecialCharacter)
            {
                return !password.All(char.IsLetterOrDigit);
            }
            return true;
        }
        internal string GenerateMixedCase(string password)
        {
            if (password.Any(char.IsUpper) && password.Any(char.IsLower))
                return password;
            
            var isLetter = false;
            var index = 0;

            if (password.Contains(" "))
            {
                index = password.IndexOf(' ') + 1;
                if (index > password.Length)
                {
                    index = 0;
                }
            }

            if (index == 0)
            {
                while (!isLetter)
                {
                    index = _rand.Next(0, password.Length - 1);
                    isLetter = char.IsLetter(password[index]);
                }

            }

            var val = char.IsUpper(password[index]) ? char.ToLower(password[index]) : char.ToUpper(password[index]);
            password = password.Remove(index, 1);
            password = password.Insert(index, val.ToString());
            return password;
        }
        internal string GenerateSpecialCharacter(string password)
        {
            var specialChars = "!@#$%()&?/>~{}*".ToCharArray();
            var isLetter = false;
            var index = 0;
            while (!isLetter)
            {
                index = _rand.Next(0, password.Length - 1);
                isLetter = char.IsLetter(password[index]);
            }

            var val = specialChars[_rand.Next(0, specialChars.Length - 1)];

            switch (password[index].ToString().ToLower())
            {
                case "e":
                    val = '@';
                    break;
                case "l":
                case "i":
                    val = '!';
                    break;
                case "s":
                    val = '$';
                    break;
                case "c":
                    val = '(';
                    break;
                case "d":
                    val = ')';
                    break;
                case "t":
                    val = '+';
                    break;
                default:
                    break;
            }
            password = password.Remove(index, 1);
            password = password.Insert(index, val.ToString());
            return password;
        }


    }
}