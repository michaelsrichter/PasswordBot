using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dx.misv.passwordbot.core.Tests
{
    [TestClass]
    public class RandomPasswordProviderWithRandomStringSourceShould
    {
        readonly IStringSource randomStringSource = new RandomStringSource();

        [TestMethod]
        public void AddLowerCaseToAnAllUpperCasePassword()
        {
            var originalPassword = "AB123456789";

            var provider = new RandomPasswordProvider(randomStringSource);

            var newPassword = provider.GenerateMixedCase(originalPassword);

            Assert.IsTrue(newPassword.Any(char.IsUpper) && newPassword.Any(char.IsLower));
        }

        [TestMethod]
        public void AddLowerCaseToAnAllUpperCasePasswordAndReplaceTheCorrectLetter()
        {
            var originalPassword = "ABCDEFGHIJKL";

            var provider = new RandomPasswordProvider(randomStringSource);

            var newPassword = provider.GenerateMixedCase(originalPassword);

            Assert.AreEqual(originalPassword, newPassword.ToUpper());
        }

        [TestMethod]
        public void ConfirmGoodPasswordHasLettersAndNumbers()
        {
            var password = "ABCDEFGHI123";
            Assert.IsTrue(RandomPasswordProvider.IsGoodPassword(password,
                new PasswordRequestOptions() {CaseType = new CaseTypeLettersAndNumbers(), SpecialCharacter = false}));
        }

        [TestMethod]
        public void ConfirmBadPasswordWhenMissingNumbers()
        {
            var password = "ABCDEFGHIJLK";
            Assert.IsFalse(RandomPasswordProvider.IsGoodPassword(password, new PasswordRequestOptions() { CaseType = new CaseTypeLettersAndNumbers(), SpecialCharacter = false }));
        }

        [TestMethod]
        public void ConfirmBadPasswordWhenMissingLetters()
        {
            var password = "123456789";
            Assert.IsFalse(RandomPasswordProvider.IsGoodPassword(password, new PasswordRequestOptions() { CaseType = new CaseTypeLettersAndNumbers(), SpecialCharacter = false }));
        }

        [TestMethod]
        public void ConfirmBadPasswordWhenMissingSpecialCharacter()
        {
            var password = "123456789";
            Assert.IsFalse(RandomPasswordProvider.IsGoodPassword(password, new PasswordRequestOptions() { CaseType = new CaseTypeLettersAndNumbers(), SpecialCharacter = true }));
        }
        [TestMethod]
        public void ConfirmGoodPasswordWhenHavingSpecialCharacter()
        {
            var password = "123456789!";
            Assert.IsFalse(RandomPasswordProvider.IsGoodPassword(password, new PasswordRequestOptions() { CaseType = new CaseTypeLettersAndNumbers(), SpecialCharacter = true }));
        }
    }
}