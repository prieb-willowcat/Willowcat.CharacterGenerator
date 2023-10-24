namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface IHttpJsonClient
    {
        string DownloadJson(string url);
        Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken = default);
    }
}