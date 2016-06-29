namespace dx.misv.passwordbot.core
{
    public interface IPasswordProvider
    {
        string GetPassword(PasswordRequestOptions options);
    }
}