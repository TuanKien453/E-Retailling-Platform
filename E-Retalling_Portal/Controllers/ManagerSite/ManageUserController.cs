﻿using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ManageUser
{
	[TypeFilter(typeof(ManagerFilter))]
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
