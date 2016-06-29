namespace dx.misv.passwordbot.core
{
    public class PasswordRequestOptions
    {
        public int MinimumLength { get; set; }
        public ICaseType CaseType { get; set; }
        public bool SpecialCharacter { get; set; }

        public PasswordRequestOptions()
        {
            CaseType = new CaseTypeLettersAndNumbers();
        }
    }
}