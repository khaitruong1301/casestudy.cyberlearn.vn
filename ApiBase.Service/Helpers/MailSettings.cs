namespace ApiBase.Service.Helpers
{
    public interface IMailSettings
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string FromAddresses { get; set; }
        string FromName { get; set; }
        string Subject { get; set; }
        string Content { get; set; }
    }

    public class MailSettings : IMailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromName { get; set; }
        public string FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}