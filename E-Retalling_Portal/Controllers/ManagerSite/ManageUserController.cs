using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ManageUser
{
    public class ManageUserController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new Context())
            {
                var userList = context.Users.ToList();
                ViewBag.Users = userList;
             
               
            }

            return View("/Views/ManageUser/ManageUser.cshtml");
        }
    }
}
