namespace dx.misv.passwordbot.app.Services
{
    public interface IWordService
    {
        bool IsSynonym(string word, string synonymTarget);
    }
}