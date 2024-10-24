using E_Retalling_Portal.Models.GHNRequestModel;
using E_Retalling_Portal.Models.GHNResponseModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace E_Retalling_Portal.Libraries
{
    public class GHNLibrary
    {
        private readonly HttpClient _httpClient;

        public GHNLibrary(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Token", token);
        }

        public async Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest, string shopId)
        {

            _httpClient.DefaultRequestHeaders.Remove("ShopId");  
            _httpClient.DefaultRequestHeaders.Add("ShopId", shopId); 

            var requestUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";

            var jsonRequest = JsonConvert.SerializeObject(orderRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(jsonResponse);
                return orderResponse;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed with status code: {response.StatusCode}, Error: {errorResponse}");
            }
        }
        public async Task<List<Province>> GetProvincesAsync()
        {
            var requestUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var provinceResponse = JsonConvert.DeserializeObject<ProvinceResponse>(jsonResponse);
                return provinceResponse.Data;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed with status code: {response.StatusCode}, Error: {errorResponse}");
            }
        }
        public async Task<List<District>> GetDistrictsAsync(int provinceId)
        {
            var requestUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district";
            var requestBody = new { province_id = provinceId };
            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var districtResponse = JsonConvert.DeserializeObject<DistrictResponse>(jsonResponse);
                return districtResponse.Data;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed with status code: {response.StatusCode}, Error: {errorResponse}");
            }
        }
        public async Task<List<Ward>> GetWardsAsync(int districtId)
        {
            var requestUrl = $"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={districtId}";
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var wardResponse = JsonConvert.DeserializeObject<WardResponse>(jsonResponse);
                return wardResponse.Data;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed with status code: {response.StatusCode}, Error: {errorResponse}");
            }
        }
    }
}
