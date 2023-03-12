namespace EtherScanTest.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<T> RequestAsync<T>(string uri, string httpMethod = "GET");
    }
}
