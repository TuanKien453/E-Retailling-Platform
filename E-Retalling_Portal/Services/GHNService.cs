using E_Retalling_Portal.Libraries;
using E_Retalling_Portal.Models.GHNResponseModel;
using Newtonsoft.Json;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.GHNRequestModel;
namespace E_Retalling_Portal.Services
{
    public class GHNService
    {
        private readonly GHNLibrary _ghnLibrary;

        public GHNService(string token)
        {
            _ghnLibrary = new GHNLibrary(token);
        }
        public async Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest, string shopId) {
            var response = await _ghnLibrary.CreateOrderAsync(orderRequest, shopId);
            return JsonConvert.DeserializeObject<OrderResponse>(response.ToString());
        }



    }
}
