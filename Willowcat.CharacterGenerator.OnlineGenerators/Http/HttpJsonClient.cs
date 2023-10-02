namespace Willowcat.CharacterGenerator.Core.Randomizer
{

    public class HttpJsonClient : IHttpJsonClient
    {
        public string DownloadJson(string url)
        {
            return DownloadJsonAsync(url).Result;
        }

        public async Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken = default)
        {
            string content = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage? response = await client.GetAsync(url, cancellationToken);
                content = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return content;
        }
    }
}
