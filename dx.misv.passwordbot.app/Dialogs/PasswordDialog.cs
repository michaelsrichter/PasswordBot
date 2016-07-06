using System;
using System.Linq;
using System.Threading.Tasks;
using dx.misv.passwordbot.app.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Practices.Unity;

namespace dx.misv.passwordbot.app.Dialogs
{
    [LuisModel("29cb07de-4dfc-42df-b15f-841d655d01fc", "7687843975f7495d9fb5471a2ac983bc")]
    [Serializable]
    public class PasswordDialog : LuisDialog<object>
    {
        private IWordService _wordService;
        internal int Count;

        [Dependency]
        public IWordService WordService
        {
            get { return _wordService; }
            set { _wordService = value; }
        }

        //[LuisIntent("Remember Password")]
        //public async Task RememberPassword(IDialogContext context, LuisResult result)
        //{
        //    return Chain.From()
        //}
        [LuisIntent("Send Password")]
        public async Task GetPassword(IDialogContext context, LuisResult result)
        {
            EntityRecommendation strength;
            EntityRecommendation number;
            if (!result.TryFindEntity("strength", out strength))
            {
                strength = new EntityRecommendation(type: "strength") {Entity = string.Empty};
            }
            if (!result.TryFindEntity("builtin.number", out number))
            {
                number = new EntityRecommendation(type: "builtin.number") {Entity = "1"};
            }

            var isDigit = int.TryParse(number.Entity, out Count);
            if (!isDigit)
            {
                Count = (int) Utility.ToLong(number.Entity);
            }
            var supportedStrength = string.Empty;
            if (!string.IsNullOrEmpty(strength.Entity))
            {
                var supportedStrengths = Utility.PasswordStrengths();

                supportedStrength = supportedStrengths.FirstOrDefault(s => _wordService.IsSynonym(strength.Entity, s));
            }


            if (!string.IsNullOrEmpty(supportedStrength))
            {
                await PrintPassword(context, supportedStrength, Count);
            }
            else
            {
                PromptDialog.Choice(context,
                    PasswordStrengthChosen,
                    Utility.PasswordStrengths(),
                    "Sure. How strong do you want your passwords?");
            }
        }

        private async Task PrintPassword(IDialogContext context, string strength, int count)
        {
            var passwords = await Utility.PasswordAPIRequest(strength, count.ToString());
            var multi = count > 1 ? "s" : string.Empty;
            await
                context.PostAsync(
                    $"You asked for {count} {strength} password{multi}. Here you go: {string.Join(", ", passwords)}");

            context.Wait(MessageReceived);
        }

        public async Task PasswordStrengthChosen(IDialogContext context, IAwaitable<string> result)
        {
            var strength = await result;
            await PrintPassword(context, strength, Count);
        }


        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.None);
            }
            else
            {
                await context.PostAsync($"MESSAGE =  {message.Text}");

                context.Wait(MessageReceived);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}