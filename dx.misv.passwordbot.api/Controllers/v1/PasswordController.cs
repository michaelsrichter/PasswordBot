using System.Collections.Generic;
using System.Web.Http;
using dx.misv.passwordbot.core;

namespace dx.misv.passwordbot.api.Controllers.v1
{
    [RoutePrefix("api/v1")]
    public class PasswordController : ApiController
    {
        private readonly IPasswordCore _passwordCore;

        public PasswordController(IPasswordCore passwordCore)
        {
            _passwordCore = passwordCore;
        }

        // GET api/values
        [Route("")]
        [HttpGet]
        public string Get()
        {
            return _passwordCore.GetPassword();
        }
        [Route("{type}/{length:int}/{count:int?}")]
        [HttpGet]
        public IEnumerable<string> Get(string type, int length, int count = 1)
        {
            var options = new PasswordRequestOptions() {CaseType = new CaseTypeLettersAndNumbers(), MinimumLength = length};

            switch (type)
            {
                case "ln":
                    options.CaseType = new CaseTypeLettersAndNumbers();
                    break;
                case "lnm":
                    options.CaseType = new CaseTypeLettersAndNumbersMixedCasing();
                    break;
                case "l":
                    options.CaseType = new CaseTypeLetters();
                    break;
                case "lm":
                    options.CaseType = new CaseTypeLettersMixedCasing();
                    break;
                case "lns":
                    options.CaseType = new CaseTypeLettersAndNumbers();
                    options.SpecialCharacter = true;
                    break;
                case "lnms":
                    options.CaseType = new CaseTypeLettersAndNumbersMixedCasing();
                    options.SpecialCharacter = true;
                    break;
                case "ls":
                    options.CaseType = new CaseTypeLetters();
                    options.SpecialCharacter = true;
                    break;
                case "lms":
                    options.CaseType = new CaseTypeLettersMixedCasing();
                    options.SpecialCharacter = true;
                    break;
            }

            return GetPasswords(options, count);
        }

        [Route("simple/{count:int?}")]
        [HttpGet]
        public IEnumerable<string> GetSimple(int count = 1)
        {
            var options = new PasswordRequestOptions {CaseType = new CaseTypeLettersMixedCasing(), MinimumLength = 6};
            return GetPasswords(options, count);
        }

        [Route("strong/{count:int?}")]
        [HttpGet]
        public IEnumerable<string> GetStrong(int count = 1)
        {
            var options = new PasswordRequestOptions
            {
                CaseType = new CaseTypeLettersAndNumbersMixedCasing(),
                MinimumLength = 8
            };
            return GetPasswords(options, count);
        }

        [Route("complex/{count:int?}")]
        [HttpGet]
        public IEnumerable<string> GetComplex(int count = 1)
        {
            var options = new PasswordRequestOptions()
            {
                CaseType = new CaseTypeLettersAndNumbersMixedCasing(),
                SpecialCharacter = true,
                MinimumLength = 10
            };
            return GetPasswords(options, count);
        }

        public IEnumerable<string> GetPasswords(PasswordRequestOptions options, int count)
        {
            if (options.MinimumLength > 15)
            {
                options.MinimumLength = 15;
            }
            var passwords = new List<string>();
            for (var i = 0; i < count; i++)
            {
                passwords.Add(_passwordCore.GetPassword(options));
            }
            return passwords;
        }
    }
}
