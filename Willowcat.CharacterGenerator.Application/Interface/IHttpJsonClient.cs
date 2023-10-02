namespace Willowcat.CharacterGenerator.Core
{
    public interface IHttpJsonClient
    {
        string DownloadJson(string url);
        Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken = default);
    }
}