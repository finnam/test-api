using System;
using System.Threading.Tasks;
using System.Net.Http;


namespace TestApi.Function
{
    public class RestApi : IRestApi
    {
        HttpClient _httpClient = new HttpClient();
        string _baseUrl;
        public RestApi(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<string> GetEndPointAsync(string ep)
        {
            string url = _baseUrl + ep;
            HttpRequestMessage usersRequest = new HttpRequestMessage(HttpMethod.Get, url);
            var res = await _httpClient.SendAsync(usersRequest);
            string json= "";
            if (res.IsSuccessStatusCode)
            {
                json = await res.Content.ReadAsStringAsync();
            }
            return json;
        }
    }
}