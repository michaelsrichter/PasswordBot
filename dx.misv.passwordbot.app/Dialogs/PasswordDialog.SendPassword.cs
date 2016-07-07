using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace dx.misv.passwordbot.app.Dialogs
{
    public partial class PasswordDialog
    {
        [LuisIntent("Send Password")]
        public async Task SendPassword(IDialogContext context, LuisResult result)
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

        public async Task PasswordStrengthChosen(IDialogContext context, IAwaitable<string> result)
        {
            var strength = await result;
            await PrintPassword(context, strength, Count);
        }

        internal async Task PrintPassword(IDialogContext context, string strength, int count)
        {
            var passwords = await Utility.PasswordAPIRequest(strength, count.ToString());
            var multi = count > 1 ? "s" : string.Empty;
            var list = passwords.ToMarkdownList();
            await
                context.PostAsync(
                    $"You asked for {count} {strength} password{multi}. Here you go: {list}");

            context.Wait(MessageReceived);
        }
    }
}