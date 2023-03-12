using EtherScanTest.Infrastructure.Exceptions;
using EtherScanTest.Services.Interfaces;
using Newtonsoft.Json;
using Polly;
using System.Net.Mime;
using System.Text;

namespace EtherScanTest.Services.Implements
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAsyncPolicy<HttpResponseMessage> _policy;
        private readonly ILogger<IHttpClientService> _logger;
        public HttpClientService(IHttpClientFactory httpClientFactory, ILogger<IHttpClientService> logger, IAsyncPolicy<HttpResponseMessage> policy)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _policy = policy;
        }

        public async Task<T> RequestAsync<T>(string uri, string httpMethod = "GET")
        {
            try
            {
                HttpRequestMessage request = null;
                using (var httpClient = _httpClientFactory.CreateClient("Etherscan"))
                {
                    
                    var response = await _policy.ExecuteAsync(() => {
                        using var requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), new Uri(uri));
                        request = requestMessage;
                        return httpClient.SendAsync(requestMessage);
                    });

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = $"Result status code: {uri} - {response.StatusCode}";
                        _logger.LogError(message);
                        throw await ApiException.CreateAsync(request, response).ConfigureAwait(false);
                    }
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(responseString, new JsonSerializerSettings()
                    {
                        Error = (sender, error) =>
                        {
                            error.ErrorContext.Handled = true;
                            _logger.LogError($"Json convert error. {responseString}");
                        }
                    });
                    return result;
                }
            }
            catch (ApiException ex)
            {
                _logger.LogError($"Call url error {ex.Message}", ex);
                var errorCode = "error";
                var response = new { status = new { code = errorCode, msg = ex.Message } };
                var message = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                _logger.LogInformation(message.Content.ToString());
                return default;
            }
        }
    }
}
