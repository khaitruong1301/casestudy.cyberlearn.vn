using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ApiBase.Service.Helpers
{
    public interface ITranslator
    {
        Task<string> TranslateText(string text, string language);

        Task<string> TranslateHtml(string text, string language);

        Task<string> TranslateObject(object listObj, string language);
    }

    public class Translator : ITranslator
    {
        private readonly TranslationClient client;

        public Translator()
        {
            client = TranslationClient.Create();
        }

        public async Task<string> TranslateHtml(string text, string language)
        {
            var response = await client.TranslateHtmlAsync(text, language);
            return response.TranslatedText;
        }

        public async Task<string> TranslateText(string text, string language)
        {
            var response = await client.TranslateTextAsync(text, language);
            return response.TranslatedText;
        }

        public async Task<string> TranslateObject(object listObj, string language)
        {
            string json = JsonConvert.SerializeObject(listObj);
            var response = await client.TranslateTextAsync(json, language);
            return response.TranslatedText;
        }
    }
}