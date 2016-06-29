using System.Linq;

namespace dx.misv.passwordbot.core
{
    public class CaseTypeLettersAndNumbers : ICaseType
    {
        public bool Check(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }
    }
}