using E_Retalling_Portal.Libraries;
using E_Retalling_Portal.Models.GHNResponseModel;
using Newtonsoft.Json;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.GHNRequestModel;
using System.Net.Http.Headers;
namespace E_Retalling_Portal.Services
{
    public class GHNService
    {
        private readonly HttpClient _httpClient;
        private readonly GHNLibrary _ghnLibrary;
        private readonly IConfiguration _configuration;
        public GHNService(string token, GHNLibrary ghnLibrary, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Token", _configuration["token"]);
            _ghnLibrary = ghnLibrary;
        }

        public async Task<List<Province>> GetProvincesAsync()
        {
            var requestUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province";
            var request = _ghnLibrary.CreateRequest(requestUrl, HttpMethod.Get);

            var jsonResponse = await GetJsonResponseAsync(request);
            return _ghnLibrary.DeserializeJsonResponse<ProvinceResponse>(jsonResponse).Data;
        }

        public async Task<List<District>> GetDistrictsAsync(int provinceId)
        {
            var requestUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district";
            var requestBody = new { province_id = provinceId };
            var request = _ghnLibrary.CreateRequest(requestUrl, HttpMethod.Post, requestBody);

            var jsonResponse = await GetJsonResponseAsync(request);
            return _ghnLibrary.DeserializeJsonResponse<DistrictResponse>(jsonResponse).Data;
        }

        public async Task<List<Ward>> GetWardsAsync(int districtId)
        {
            var requestUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id";
            var requestBody = new { district_id = districtId };
            var request = _ghnLibrary.CreateRequest(requestUrl, HttpMethod.Post, requestBody);

            var jsonResponse = await GetJsonResponseAsync(request);
            return _ghnLibrary.DeserializeJsonResponse<WardResponse>(jsonResponse).Data;
        }

        private async Task<string> GetJsonResponseAsync(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed with status code: {response.StatusCode}, Error: {errorResponse}");
            }
            return await response.Content.ReadAsStringAsync();
        }

    }
}
