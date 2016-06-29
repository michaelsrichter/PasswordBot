using System.Linq;

namespace dx.misv.passwordbot.core
{
    public class CaseTypeNumbers : ICaseType
    {
        public bool Check(string password)
        {
            return password.All(char.IsDigit);
        }
    }
}