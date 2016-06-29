using System.Linq;

namespace dx.misv.passwordbot.core
{
    public class CaseTypeLettersMixedCasing : ICaseType
    {
        public bool Check(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) && !password.Any(char.IsDigit);
        }
    }
}