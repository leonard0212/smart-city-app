using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services.Common
{
    public interface IHttpClientService
    {
        Task<(HttpResponseMessage, TResponse)> CallApi<TRequest, TResponse>(TRequest request
          , string baseUrl
          , string url
          , string bearerAccessToken = null
          , string method = "POST"
          , Dictionary<string, string> headers = null);

        Task<byte[]> ProcessAiWithFormData(string actionUrl, byte[] paramFileBytes, string json);
        Task<string> GetDataFromFileAiWithFormData(string actionUrl, byte[] paramFileBytes);
        Task<string> ProcessAiWithFormDataStringResult(string actionUrl, byte[] paramFileBytes, string json);
        Task<byte[]> GetImageBytesAsync(string url);
    }
}
