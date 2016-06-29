using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dx.misv.passwordbot.core.Tests
{
    [TestClass]
    public class PasswordCoreWithRandomPasswordProviderAndWordStringSourceShould
    {
        readonly IStringSource wordStringSource = new WordStringSource();
        [TestMethod]
        public void GenerateAPassword()
        {
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword();
            Assert.IsNotNull(password);
        }

        [TestMethod]
        public void GenerateAPasswordWithMinimumLength()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions {MinimumLength = minLength};
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.Length >= 8);
        }

        [TestMethod]
        public void GenerateAPasswordWithMixedCase()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions {MinimumLength = minLength, CaseType = new CaseTypeLettersAndNumbersMixedCasing()};
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.Any(char.IsUpper) && password.Any(char.IsLower));
        }

        [TestMethod]
        public void GenerateAPasswordWithLettersOnlyButIgnoreSpaces()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLetters() };
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(Regex.Replace(password, @"\s+", "").All(char.IsLetter));
        }

        [TestMethod]
        public void GenerateAPasswordWithMixedCaseLettersOnly()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLettersMixedCasing() };
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(Regex.Replace(password, @"\s+", "").All(char.IsLetter));
        }

        [TestMethod]
        public void GenerateAPasswordWithSpecialCharacters()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLettersAndNumbersMixedCasing(), SpecialCharacter = true};
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(!password.All(char.IsLetterOrDigit));
        }

        [TestMethod]
        public void GenerateAPasswordWithoutspaces()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLettersAndNumbersMixedCasing(), SpecialCharacter = true };
            var core = new PasswordCore(new RandomPasswordProvider(wordStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(!password.All(char.IsWhiteSpace));
        }
    }
}