namespace Notification.Consumer.Models
{
    public class Email
    {
        public string Name { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string PlainTextContext { get; set; }
        public string HtmlContent { get; set; }
    }
}
