namespace dx.misv.passwordbot.core
{
    public interface IPasswordCore
    {
        string GetPassword();
        string GetPassword(PasswordRequestOptions options);
    }
}