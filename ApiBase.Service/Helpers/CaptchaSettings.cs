namespace ApiBase.Service.Helpers
{
    public interface ICaptchaSettings
    {
        string CaptchaSecret { get; set; }
    }

    public class CaptchaSettings : ICaptchaSettings
    {
        public string CaptchaSecret { get; set; }
    }
}