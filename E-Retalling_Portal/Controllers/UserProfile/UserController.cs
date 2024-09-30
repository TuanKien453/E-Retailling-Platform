using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;

using static System.Net.Mime.MediaTypeNames;

namespace E_Retalling_Portal.Controllers.UserProfile
{
    public class UserController : Controller
    {


        public IActionResult ViewProfile()
        {
            using (var context = new Context())
            {
                User user = context.Users.getUserById(1).FirstOrDefault();
                Account account = context.Accounts.getAccountByUserId(1).FirstOrDefault();
                
                ViewBag.User = user;
                ViewBag.Account = account;

                return View();
            }

        }

        [HttpPost]
        public IActionResult SavePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            using (var context = new Context())
            {
                Account account = context.Accounts.getAccountByUserId(1).FirstOrDefault();

                if (account.password != currentPassword)
                {
                    TempData["ErrorMessage"] = "The current password is incorrect.";
                    return RedirectToAction("ViewProfile");
                }

                if (newPassword != confirmPassword)
                {
                    TempData["ErrorMessage"] = "The new password and confirmation do not match.";
                    return RedirectToAction("ViewProfile");
                }

                account.password = newPassword;
                context.SaveChanges();

                TempData["SuccessMessage"] = "The password has been changed successfully.";
                return RedirectToAction("ViewProfile");
            }
        }

        [HttpPost]
        public IActionResult Update(User user)
        {
            using (var context = new Context())
            {
                

                User  userProfile = context.Users.getUserById(1).FirstOrDefault();
                Account accountProfile = context.Accounts.getAccountByUserId(1).FirstOrDefault();

                userProfile.displayName = user.displayName;
                userProfile.email = user.email;
                userProfile.phoneNumber = user.phoneNumber;
                userProfile.birthday = user.birthday;
                userProfile.gender = user.gender;
                userProfile.firstName = user.firstName;
                userProfile.lastName = user.lastName;
                userProfile.address = user.address; 
                context.SaveChanges();


                    TempData["UpdateMessage"] = "UpdateSuccessfully!";
                    return RedirectToAction("ViewProfile");
                }
                
            }

        }
    }

