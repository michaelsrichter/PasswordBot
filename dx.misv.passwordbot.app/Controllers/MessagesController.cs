using System.Threading.Tasks;
using System.Web.Http;
using dx.misv.passwordbot.app.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace dx.misv.passwordbot.app.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        ///     POST: api/Messages
        ///     Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody] Message message)
        {
            if (message.Type == "Message")
            {
                // return our reply to the user
                return
                    await
                        Conversation.SendAsync(message,
                            () => (IDialog<object>) Configuration.DependencyResolver.GetService(typeof(PasswordDialog)));
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
}