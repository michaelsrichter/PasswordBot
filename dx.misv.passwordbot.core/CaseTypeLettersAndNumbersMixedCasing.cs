using System.Linq;

namespace dx.misv.passwordbot.core
{
    public class CaseTypeLettersAndNumbersMixedCasing : ICaseType
    {
        public bool Check(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsDigit) && password.Any(char.IsUpper) &&
                   password.Any(char.IsLower);
        }
    }
}