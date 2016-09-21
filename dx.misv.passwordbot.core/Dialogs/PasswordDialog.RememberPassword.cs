using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;

namespace dx.misv.passwordbot.core.Dialogs
{
    public partial class PasswordDialog
    {
        [LuisIntent("Remember Password")]
        public async Task RememberPassword(IDialogContext context, LuisResult result)
        {
            context.Call(FormDialog.FromForm(PasswordRecord.BuildForm, FormOptions.PromptInStart),
                PasswordRecordFormComplete);
        }

        internal async Task PasswordRecordFormComplete(IDialogContext context, IAwaitable<PasswordRecord> result)
        {
            var r = await result;
            await context.PostAsync("I'll remember this password for " + r.Service);
            context.Wait(MessageReceived);
        }
    }
}