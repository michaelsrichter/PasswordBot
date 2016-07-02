using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dx.misv.passwordbot.app.Controllers;
using dx.misv.passwordbot.app.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using RestSharp;

namespace dx.misv.passwordbot.app.Dialogs
{
    [LuisModel("29cb07de-4dfc-42df-b15f-841d655d01fc", "7687843975f7495d9fb5471a2ac983bc")]
    [Serializable]
    public class PasswordDialog : LuisDialog<object>
    {
        private IWordService _wordService;

        [Microsoft.Practices.Unity.Dependency]
        public IWordService WordService
        {
            get { return _wordService; }
            set { _wordService = value; }
        }

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

            int digit;
            var isDigit = int.TryParse(number.Entity, out digit);
            if (!isDigit)
            {
                digit = (int) Utility.ToLong(number.Entity);
            }
            var supportedStrength = "strong";
            if (!string.IsNullOrEmpty(strength.Entity))
            {
                var supportedStrengths = new[] { "simple", "strong", "complex" };

                supportedStrength = supportedStrengths.FirstOrDefault(s => _wordService.IsSynonym(strength.Entity, s));
            }


            if (!string.IsNullOrEmpty(supportedStrength))
            {
                var client = new RestClient("http://localhost:1606");
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest($"api/v1/{supportedStrength}", Method.GET);
                var restResponse = await client.ExecuteTaskAsync<List<string>>(request);
                await context.PostAsync($"You asked for {digit} {supportedStrength} password: {restResponse.Data[0]}");
            }
            else
            {
                await context.PostAsync($"You asked for {digit} {supportedStrength} password: ");
            }

            context.Wait(MessageReceived);
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