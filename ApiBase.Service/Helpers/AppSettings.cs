namespace ApiBase.Service.Helpers
{
    public interface IAppSettings
    {
        string Secret { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        // Thuộc tính Secret sẽ mapping với 
        // key Secret khai báo ở appsettings.json
        public string Secret { get; set; }
    }
}