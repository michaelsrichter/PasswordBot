using System;
using dx.misv.passwordbot.app.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Practices.Unity;

namespace dx.misv.passwordbot.app.Dialogs
{
    [LuisModel("29cb07de-4dfc-42df-b15f-841d655d01fc", "7687843975f7495d9fb5471a2ac983bc")]
    [Serializable]
    public partial class PasswordDialog : LuisDialog<object>
    {
        private IWordService _wordService;
        internal int Count;

        [Dependency]
        public IWordService WordService
        {
            get { return _wordService; }
            set { _wordService = value; }
        }
    }
}