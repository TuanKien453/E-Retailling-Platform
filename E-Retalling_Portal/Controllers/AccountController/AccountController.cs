using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Retalling_Portal.Models.Query;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Controllers.AccountController
{
    public class AccountController : Controller
    {
        public IActionResult NormalRegister()
        {
            return View();
        }
        public IActionResult RegisterCheck()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string email = Request.Form["email"];
            string phoneNumber = Request.Form["phoneNumber"];
            string displayName = Request.Form["displayName"];
            string birthday = Request.Form["birthday"];
            string gender = Request.Form["gender"];
            string firstName = Request.Form["firstName"];
            string lastName = Request.Form["lastName"];
            string passwordConfirm = Request.Form["passwordConfirm"];
            string address = Request.Form["address"];            
            using (var context = new Context())
            {
                
                User testUser = context.Users.CheckUserData(email, phoneNumber).FirstOrDefault();
                Account testAcc = context.Accounts.CheckAccount(username).FirstOrDefault();
                if (testUser == null && testAcc == null && password==passwordConfirm)
                {
                    User newUser = new User { email = email, phoneNumber = phoneNumber, displayName = displayName, birthday = birthday, gender = gender, firstName = firstName, lastName = lastName };
                    context.Add(newUser);
                    context.SaveChanges();
                    newUser = context.Users.GetUserIdByEmail(email).FirstOrDefault();
                    Account newAccount = new Account{ userId = newUser.id, username = username, password = password, roleId = 1, externalId = 0, externalType = "normal" };
                    context.Add(newAccount);
                    context.SaveChanges();
                    Address newAddress = new Address {address = address, userId = newUser.id};
                    context.Add(newAddress);
                    context.SaveChanges();
                    return RedirectToAction("RegisterSucceed");
                } else
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

                    return View("NormalRegister"); ;
                }
            }
        }
        public IActionResult RegisterSucceed()
        {
            return View();
        }

        
    }
}
