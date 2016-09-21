namespace dx.misv.passwordbot.core.Services
{
    public interface IWordService
    {
        bool IsSynonym(string word, string synonymTarget);
    }
}