using E_Retalling_Portal.Models.GHNRequestModel;
using E_Retalling_Portal.Models.GHNResponseModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace E_Retalling_Portal.Libraries
{
    public class GHNLibrary
    {
        public HttpRequestMessage CreateRequest(string url, HttpMethod method, object body = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (body != null)
            {
                var jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            return request;
        }

        public T DeserializeJsonResponse<T>(string jsonResponse)
        {
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

    }
}
