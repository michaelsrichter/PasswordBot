using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using dx.misv.passwordbot.app.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace dx.misv.passwordbot.app.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IWordService _wordService;

        public MessagesController(IWordService wordService)
        {
            _wordService = wordService;
        }

        /// <summary>
        ///     POST: api/Messages
        ///     Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody] Message message)
        {
            if (message.Type == "Message")
            {
                // return our reply to the user
                return await Conversation.SendAsync(message, () => new PasswordDialog(_wordService));
            }
            return HandleSystemMessage(message);
        }


        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                var reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }

    [LuisModel("29cb07de-4dfc-42df-b15f-841d655d01fc", "7687843975f7495d9fb5471a2ac983bc")]
    [Serializable]
    public class PasswordDialog : LuisDialog<object>
    {
        private readonly IWordService _wordService;

        public PasswordDialog(IWordService wordService)
        {
            _wordService = wordService;
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


            await context.PostAsync($"You asked for {digit} {supportedStrength} password: ");

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

    public static class Utility
    {
        private static readonly Dictionary<string, long> numberTable =
            new Dictionary<string, long>
            {
                {"zero", 0},
                {"one", 1},
                {"two", 2},
                {"three", 3},
                {"four", 4},
                {"five", 5},
                {"six", 6},
                {"seven", 7},
                {"eight", 8},
                {"nine", 9},
                {"ten", 10},
                {"eleven", 11},
                {"twelve", 12},
                {"thirteen", 13},
                {"fourteen", 14},
                {"fifteen", 15},
                {"sixteen", 16},
                {"seventeen", 17},
                {"eighteen", 18},
                {"nineteen", 19},
                {"twenty", 20},
                {"thirty", 30},
                {"forty", 40},
                {"fifty", 50},
                {"sixty", 60},
                {"seventy", 70},
                {"eighty", 80},
                {"ninety", 90},
                {"hundred", 100},
                {"thousand", 1000},
                {"million", 1000000},
                {"billion", 1000000000},
                {"trillion", 1000000000000},
                {"quadrillion", 1000000000000000},
                {"quintillion", 1000000000000000000}
            };

        public static long ToLong(string numberString)
        {
            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                .Select(m => m.Value.ToLowerInvariant())
                .Where(v => numberTable.ContainsKey(v))
                .Select(v => numberTable[v]);
            long acc = 0, total = 0L;
            foreach (var n in numbers)
            {
                if (n >= 1000)
                {
                    total += acc*n;
                    acc = 0;
                }
                else if (n >= 100)
                {
                    acc *= n;
                }
                else acc += n;
            }
            return (total + acc)*(numberString.StartsWith("minus",
                StringComparison.InvariantCultureIgnoreCase)
                ? -1
                : 1);
        }
    }
}