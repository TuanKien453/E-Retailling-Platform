using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Retalling_Portal.Models.Query;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Controllers.AccountController
{
    public class AccountController : Controller
    {
        public IActionResult NormalRegister([DataType(DataType.EmailAddress)]String email)
        {
            return View();
        }
        public IActionResult RegisterCheck([DataType(DataType.EmailAddress),MaxLength(100)] String email,[MaxLength(100)] String username, [DataType(DataType.Password),MaxLength(100)] String password,
                                            [DataType(DataType.PhoneNumber), MaxLength(15)]String phoneNumber, [MaxLength(100)] String displayName, [DataType(DataType.Date), MaxLength(20)] String birthday,
                                            [MaxLength(15)] String gender, [MaxLength(100)] String firstName, [MaxLength(100)] String lastName, [DataType(DataType.Password), MaxLength(100)] String passwordConfirm,
                                            [MaxLength(100)] String address)
        {
            if(ModelState.IsValid == true)
            {
                using (var context = new Context())
                {

                    User testUser = context.Users.GetVaildUserData(email, phoneNumber).FirstOrDefault();
                    Account testAcc = context.Accounts.GetVaildAccount(username).FirstOrDefault();
                    if (testUser == null && testAcc == null && password == passwordConfirm)
                    {
                        User newUser = new User { email = email, phoneNumber = phoneNumber, displayName = displayName, birthday = birthday, gender = gender, firstName = firstName, lastName = lastName, address = address};
                        context.Add(newUser);
                        context.SaveChanges();
                        newUser = context.Users.GetUserIdByEmail(email).FirstOrDefault();
                        Account newAccount = new Account { userId = newUser.id, username = username, password = password, roleId = 1, externalId = null, externalType = null };
                        context.Add(newAccount);
                        context.SaveChanges();
                        return RedirectToAction("RegisterSucceed");
                    }
                    else
                    {
                        ViewBag.DisplayName = displayName;
                        ViewBag.PhoneNumber = phoneNumber;
                        ViewBag.Birthday = birthday;
                        ViewBag.Gender = gender;
                        ViewBag.FirstName = firstName;
                        ViewBag.LastName = lastName;
                        ViewBag.Address = address;
                        ViewBag.Password = password;
                        ViewBag.PasswordConfirm = passwordConfirm;
                        ViewBag.Email = email;
                        ViewBag.Phone = phoneNumber;
                        ViewBag.Username = username;
                        if (testUser != null)
                        {
                            if (testUser.email == email)
                            {
                                ViewBag.ErrorEmail = "Email is already been register ";
                            }

                            if (testUser.phoneNumber == phoneNumber)
                            {
                                ViewBag.ErrorPhone = "PhoneNumber is already registered.";
                            }

                        }
                        if (testAcc != null)
                        {
                            if (testAcc.username == username)
                            {
                                ViewBag.ErrorUsername = "This UserName is already been used.";
                            }

                        }
                        if (passwordConfirm != password)
                        {
                            ViewBag.ErrorPasswordConfirm = "Password and PasswordConfirm does not match";
                        }

                        return View("NormalRegister");
                    }
                }
            } else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }         
            
        }
        public IActionResult RegisterSucceed()
        {
            return View();
        }

        
    }
}
