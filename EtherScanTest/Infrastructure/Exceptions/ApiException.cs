using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EtherScanTest.Infrastructure.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public HttpMethod RequestMethod { get; }
        public Uri RequestUri { get; }
        public HttpStatusCode StatusCode { get; }
        public string Content { get; }
        public string ReasonPhrase { get; }

        public bool HasContent
        {
            get { return !String.IsNullOrWhiteSpace(this.Content); }
        }

        public ApiException(HttpRequestMessage request, HttpResponseMessage response, string contentString)
               : this(request.Method,
                     request.RequestUri,
                     response.StatusCode,
                     response.ReasonPhrase,
                     contentString)
        {
        }
        public ApiException(
               HttpMethod requestMethod,
               Uri requestUri,
               HttpStatusCode statusCode,
               string reasonPhrase,
               string contentString)
               : base($"{requestMethod} \"{requestUri}\" {contentString}\" {(int)statusCode}.")
        {
            this.RequestMethod = requestMethod;
            this.Data[nameof(this.RequestMethod)] = requestMethod.Method;

            this.RequestUri = requestUri;
            this.Data[nameof(this.RequestUri)] = requestUri;

            this.StatusCode = statusCode;
            this.Data[nameof(this.StatusCode)] = statusCode;

            this.ReasonPhrase = reasonPhrase;
            this.Data[nameof(this.ReasonPhrase)] = reasonPhrase;

            this.Content = contentString;
        }

        public static async Task<ApiException> CreateAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (response.Content == null)
            {
                return new ApiException(request, response, "failed with status code:");
            }

            HttpContentHeaders contentHeaders = null;
            string contentString = null;

            try
            {
                contentHeaders = response.Content.Headers;
                using (var content = response.Content)
                {
                    contentString = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch
            { } // Don't want to hide the original exception with a new one

            return new ApiException(request, response, contentString);
        }
    }
}
