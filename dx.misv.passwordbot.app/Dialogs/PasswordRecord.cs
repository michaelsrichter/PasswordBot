using System;
using Microsoft.Bot.Builder.FormFlow;

namespace dx.misv.passwordbot.app.Dialogs
{
    [Serializable]
    public class PasswordRecord
    {
        public string Password { get; set; }
        public string Service { get; set; }
        public string Username { get; set; }
        public string Note { get; set; }
        public string Project { get; set; }
        public static IForm<PasswordRecord> BuildForm()
        {
            return new FormBuilder<PasswordRecord>()
                .Message("Sure, I'll remember your password for dev/test. (don't trust me with your production data!)")
                .Build();
        }
    }
}