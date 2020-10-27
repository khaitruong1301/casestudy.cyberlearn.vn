namespace ApiBase.Service.Helpers
{
    public interface IFtpSettings
    {
        string IP { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string UrlServer { get; set; }
    }

    public class FtpSettings : IFtpSettings
    {
        public string IP { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UrlServer { get; set; }
    }
}