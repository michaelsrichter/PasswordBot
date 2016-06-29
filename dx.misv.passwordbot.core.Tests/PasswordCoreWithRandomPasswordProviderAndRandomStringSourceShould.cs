using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dx.misv.passwordbot.core.Tests
{
    [TestClass]
    public class PasswordCoreWithRandomPasswordProviderAndRandomStringSourceShould
    {
        readonly IStringSource randomStringSource = new RandomStringSource();
        [TestMethod]
        public void GenerateAPassword()
        {
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword();
            Assert.IsNotNull(password);
        }

        [TestMethod]
        public void GenerateAPasswordWithMinimumLength()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions {MinimumLength = minLength};
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.Length >= 8);
        }

        [TestMethod]
        public void GenerateAPasswordWithMixedCase()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions {MinimumLength = minLength, CaseType = new CaseTypeLettersAndNumbersMixedCasing()};
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.Any(char.IsUpper) && password.Any(char.IsLower));
        }

        [TestMethod]
        public void GenerateAPasswordWithLettersOnly()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLetters() };
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.All(char.IsLetter));
        }

        [TestMethod]
        public void GenerateAPasswordWithMixedCaseLettersOnly()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLettersMixedCasing() };
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.All(char.IsLetter));
        }

        [TestMethod]
        public void GenerateAPasswordWithNumbersOnly()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeNumbers() };
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(password.All(char.IsDigit));
        }

        [TestMethod]
        public void GenerateAPasswordWithSpecialCharacters()
        {
            var minLength = 8;
            var options = new PasswordRequestOptions { MinimumLength = minLength, CaseType = new CaseTypeLettersAndNumbersMixedCasing(), SpecialCharacter = true};
            var core = new PasswordCore(new RandomPasswordProvider(randomStringSource));
            var password = core.GetPassword(options);
            Assert.IsNotNull(password);
            Assert.IsTrue(!password.All(char.IsLetterOrDigit));
        }
    }
}