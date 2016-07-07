using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace dx.misv.passwordbot.app.Dialogs
{
    public partial class PasswordDialog
    {
        [LuisIntent("None")]
        public async Task NoIntent(IDialogContext context, LuisResult result)
        {
            await
                context.PostAsync(
                    "Hey there. I\'m the password bot. I can generate passwords for you. Try asking for two strong passwords!");
            await context.PostAsync(
                "I can also remember your passwords too so you can find them later!");
            await context.PostAsync(
                "I'm not the most secure bot, so only use these passwords for dev/test or temporary uses.");
            context.Wait(MessageReceived);
        }
    }
}