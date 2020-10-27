namespace ApiBase.Service.Helpers
{
    public interface IFacebookSettings
    {
        string AppId { get; set; }
        string AppSecret { get; set; }
    }

    public class FacebookSettings : IFacebookSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}