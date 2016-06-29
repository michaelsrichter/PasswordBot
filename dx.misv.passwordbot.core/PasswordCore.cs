namespace dx.misv.passwordbot.core
{
    public class PasswordCore : IPasswordCore
    {
        private readonly IPasswordProvider _passwordProvider;

        public PasswordCore(IPasswordProvider passwordProvider)
        {
            _passwordProvider = passwordProvider;
        }

        public string GetPassword(PasswordRequestOptions options)
        {
            return _passwordProvider.GetPassword(options);
        }

        public string GetPassword()
        {
            return
                _passwordProvider.GetPassword(new PasswordRequestOptions
                {
                    MinimumLength = 8,
                    CaseType = new CaseTypeLettersAndNumbers()
                });
        }
    }
}