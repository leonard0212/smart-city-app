using SmartCity.Core.Utils;
using SmartCity.Interfaces.Services.Common;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace SmartCity.Core.Services.Common
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(HttpResponseMessage, TResponse)> CallApi<TRequest, TResponse>(TRequest request
            , string baseUrl
            , string url
            , string bearerAccessToken = null
            , string method = "POST"
            , Dictionary<string, string> headers = null)
        {
            var httpClient = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");

            var queryParams = new List<string>();
            if (method == "GET")
            {
                Type type = request.GetType();
                var props = new List<PropertyInfo>(type.GetProperties());

                foreach (var prop in props)
                {
                    var propValue = prop.GetValue(request, null);
                    if (propValue != null)
                        queryParams.Add($"{prop.Name}={propValue}");
                }
            }

            var urll = baseUrl;
            if (!string.IsNullOrEmpty(url))
                urll += $"/{url}";
            if (queryParams.Any())
                urll += $"?{string.Join("&", queryParams)}";


            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(bearerAccessToken))
                httpClient.DefaultRequestHeaders.Add("Authorization", bearerAccessToken);

            if (headers != null)
            {
                foreach (var header in headers)
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            CertificateUtils.SetCertificatePolicy();
            HttpResponseMessage response = null;
            switch (method)
            {
                case "POST":
                    response = await httpClient.PostAsync(urll, httpContent);
                    break;
                case "GET":
                    response = await httpClient.GetAsync(urll);
                    break;
                case "PUT":
                    response = await httpClient.PutAsync(urll, httpContent);
                    break;
                case "DELETE":
                    response = await httpClient.DeleteAsync(urll);
                    break;
            }
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<TResponse>(response.Content.ReadAsStringAsync().Result);
            return (response, resp);

        }


        public async Task<byte[]> ProcessAiWithFormData(string actionUrl, byte[] paramFileBytes, string json)
        {
            var bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {

                client.Timeout = TimeSpan.FromSeconds(60);
                formData.Add(bytesContent, "image_file", "image_file.jpg");
                if (!string.IsNullOrEmpty(json))
                {
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    formData.Add(httpContent, "detections");
                }



                var response = await client.PostAsync(actionUrl, formData);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("ProcessAiWithFormData");

                var resp = await response.Content.ReadAsByteArrayAsync();
                //return resp.Replace("```json", "").Replace("```", "");
                return resp;
            }
        }

        public async Task<string> ProcessAiWithFormDataStringResult(string actionUrl, byte[] paramFileBytes, string json)
        {
            var bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                formData.Add(bytesContent, "image_file", "image_file.jpg");
                if (!string.IsNullOrEmpty(json))
                {
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    formData.Add(httpContent, "detections");
                }



                var response = await client.PostAsync(actionUrl, formData);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("ProcessAiWithFormDataStringResult");


                var resp = await response.Content.ReadAsStringAsync();
                //return resp.Replace("```json", "").Replace("```", "");
                return resp;
            }
        }


        public async Task<string> GetDataFromFileAiWithFormData(string actionUrl, byte[] paramFileBytes)
        {
            var bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                formData.Add(bytesContent, "file", "file.jpg");




                var response = await client.PostAsync(actionUrl, formData);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("GetDataFromFileAiWithFormData");

                var resp = await response.Content.ReadAsStringAsync();
                //return resp.Replace("```json", "").Replace("```", "");
                return resp;
            }
        }

        public async Task<byte[]> GetImageBytesAsync(string url)
        {

            using (HttpClient client = new HttpClient())
            {
                // Optional: Set timeout or headers
                client.Timeout = TimeSpan.FromSeconds(30);

                byte[] imageBytes = await client.GetByteArrayAsync(url);
                return imageBytes;
            }

        }
    }
}
