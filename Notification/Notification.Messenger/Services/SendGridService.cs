using Microsoft.Extensions.Options;
using Notification.Commom.Settings;
using Notification.Messenger.Interfaces;
using Notification.Messenger.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Notification.Messenger.Services
{
    public class SendGridService : IMessengerService<Email>
    {
        private readonly SendGridConfig _config;

        public SendGridService(IOptions<SendGridConfig> options)
        {
            _config = options.Value;
        }

        public async Task<bool> Deliver(Email email)
        {
            try
            {
                SendGridClient client = new(_config.Key);
                EmailAddress from = new(_config.From, _config.Name);
                EmailAddress to = new(email.Recipient, email.Name);
                SendGridMessage message = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextContext, email.HtmlContent);
                var response = await client.SendEmailAsync(message);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                throw;
            }
        }
    }
}
