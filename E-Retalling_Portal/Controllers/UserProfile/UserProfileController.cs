using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Net.Mime.MediaTypeNames;

namespace E_Retalling_Portal.Controllers.UserProfile
{
    public class UserProfileController : Controller
    {


        public IActionResult ViewProfile()
        {
            using (var context = new Context())
            {
                int? accountId = (int)HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Account account = context.Accounts.GetAccountByAccountId(accountId.Value).FirstOrDefault();
                int userId = account.userId;

                User user = context.Users.GetUserByUserIdInAccount(userId).FirstOrDefault();

                
                ViewBag.User = user;
            
                ViewBag.Account = account;



                return View();
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            using (var context = new Context())
            {
                int? accountId = (int)HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());

                Account account = context.Accounts.GetAccountByAccountId(accountId.Value).FirstOrDefault();


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

                int? accountId = (int)HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());

                User newUser = context.Users.GetUserByUserIdInAccount(accountId.Value).FirstOrDefault();

                User testUser = context.Users.GetValidUserData(user.email, user.phoneNumber, newUser.id).FirstOrDefault();

                if (testUser != null)
                {
                    newUser.displayName = user.displayName;
                    
                    newUser.birthday = user.birthday;
                    newUser.gender = user.gender;
                    newUser.firstName = user.firstName;
                    newUser.lastName = user.lastName;
                    newUser.address = user.address;
                    
                    if (testUser.email == user.email)
                    {
                        TempData["ErrorEmail"] = "Email is already been registered";
                    }

                    if (testUser.phoneNumber == user.phoneNumber)
                    {
                        TempData["ErrorPhone"] = "PhoneNumber is already registered.";
                    }
                    if(testUser.email != user.email && testUser.phoneNumber != user.phoneNumber)
                    {
                        newUser.email = user.email;
                        newUser.phoneNumber = user.phoneNumber;
                    }
                    context.SaveChanges();
                    return RedirectToAction("ViewProfile");
                }
                else
                {
                    newUser.displayName = user.displayName;

                    newUser.email = user.email;

                    newUser.phoneNumber = user.phoneNumber;
                    newUser.birthday = user.birthday;
                    newUser.gender = user.gender;
                    newUser.firstName = user.firstName;
                    newUser.lastName = user.lastName;
                    newUser.address = user.address;
                    context.SaveChanges();

                    TempData["UpdateMessage"] = "Updated Successfully!";
                    return RedirectToAction("ViewProfile");
                   
                }
                 
                
            }

        }

    }
}

