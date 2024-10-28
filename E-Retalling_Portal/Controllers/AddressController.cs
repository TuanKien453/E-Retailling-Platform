using E_Retalling_Portal.Services.ExtendService;
using E_Retalling_Portal.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers
{
	public class AddressController : Controller
	{
		private readonly GHNService _ghnService;
		public AddressController(GHNService ghnService)
		{
			_ghnService = ghnService;
		}

		public async Task<IActionResult> getProvinces()
		{
			var result = await _ghnService.GetProvincesAsync();
			var enableProvince = result.Where(p => p.IsEnable==true);
			return Json(enableProvince);
		}
		public async Task<IActionResult> getDistricts(int provinceId)
		{
			var result = await _ghnService.GetDistrictsAsync(provinceId);
			var enableDistrict = result.Where(d => d.IsEnable == true);
			return Json(enableDistrict);
		}
		public async Task<IActionResult> getWards(int districtId)
		{
			var result = await _ghnService.GetWardsAsync(districtId);
			return Json(result);
		}
	}
}
