using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ManagerSite
{
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new Context())
            {
                var settingFee = context.Settings.getSettingByID(1).FirstOrDefault();
                ViewBag.SettingFee = settingFee;
                return View("Views/ManagerSite/Setting/Index.cshtml");
            }
        }

        public IActionResult UpdateFee(int id, string name, string value)
        {
            using (var context = new Context())
            {
                int numericValue;
                if (value.EndsWith("%") && int.TryParse(value.TrimEnd('%'), out numericValue))
                {
                    if (numericValue >= 1 && numericValue <= 99)
                    {
                        var fee = context.Settings.getSettingByID(id).FirstOrDefault();
                        fee.name = name;
                        fee.value = value;
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                var settingFee = context.Settings.getSettingByID(id).FirstOrDefault();
                ViewBag.errorFeeValue = "Value must be between 1% and 99%";
				ViewBag.SettingFee = settingFee;
				return View("Views/ManagerSite/Setting/Index.cshtml");
            }
        }
    }
}
