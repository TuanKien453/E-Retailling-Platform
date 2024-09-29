using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.ViewModel;

using static System.Net.Mime.MediaTypeNames;

namespace E_Retalling_Portal.Controllers.User
{
    public class UserController : Controller
    {


        public IActionResult ViewProfile()
        {
            using (var context = new Context())
            {
                var user = context.Users.FirstOrDefault(u => u.id == 1);
                var address = context.Addresses.FirstOrDefault(a => a.userId == 1);
                var account = context.Accounts.FirstOrDefault(a => a.userId == 1);

                var viewModel = new UserProfileVM
                {
                    User = user,
                    Address = address,
                    Account = account
                };

                return View(viewModel);
            }

        }

        [HttpPost]
        public IActionResult SavePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            using (var context = new Context())
            {
                var account = context.Accounts.FirstOrDefault(u => u.userId == 1);

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
        public IActionResult Update(UserProfileVM model)
        {
            using (var context = new Context())
            {
                var user = context.Users.FirstOrDefault(u => u.id == 1);
                var address = context.Addresses.FirstOrDefault(a => a.userId == 1);
                var account = context.Accounts.FirstOrDefault(a => a.userId == 1);
                if (model.User != null && model.Address != null)
                {
                    user.displayName = model.User.displayName;
                    user.email = model.User.email;
                    user.phoneNumber = model.User.phoneNumber;
                    user.birthday = model.User.birthday;
                    user.gender = model.User.gender;
                    user.firstName = model.User.firstName;
                    user.lastName = model.User.lastName;
                    address.address = model.Address.address;

                    context.SaveChanges();


                    TempData["UpdateMessage"] = "UpdateSuccessfully!";
                    return RedirectToAction("ViewProfile");
                }
                else
                {
                    return View("ViewProfile");
                }
            }

        }
    }
}
