using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Retalling_Portal.Models.Query;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using E_Retalling_Portal.Models.Enums;
namespace E_Retalling_Portal.Controllers.AccountController
{
    public class RegisterController : Controller
    {
        public IActionResult CustomerEmailRegister()
        {
            HttpContext.Session.SetInt32(SessionKeys.RoleID.ToString(), 1);
            return View();
        }
        public IActionResult CustomerEmailRegisterCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email, [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    HttpContext.Session.SetString(SessionKeys.Email.ToString(), email);
                    HttpContext.Session.SetString(SessionKeys.PhoneNumber.ToString(), phoneNumber);
                    User phoneUser = context.Users.GetValidUserDataByPhone(phoneNumber).FirstOrDefault();
                    User emailUser = context.Users.GetValidUserDataByEmail(email).FirstOrDefault();
                    if (phoneUser != null && emailUser == null)
                    {
                        ViewBag.Email = email;
                        ViewBag.Phone = phoneNumber;
                        if (phoneUser != null)
                        {
                            ViewBag.ErrorPhone = "This Phone Number has already been register";
                        }
                        return View("CustomerEmailRegister");

                    }
                    else
                    {
                        if (phoneUser == null && emailUser != null)
                        {
                            ViewBag.Email = email;
                            ViewBag.Phone = phoneNumber;
                            if (emailUser != null)
                            {
                                ViewBag.ErrorEmail = "This Email has already been register";
                            }
                            return View("CustomerEmailRegister");
                        }
                    }
                    User? testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                    if (testUser == null)
                    {
                        return RedirectToAction("UserInformationRegister");
                    }
                    else
                    {
                        Account testAccount = context.Accounts.GetValidAccountByUserId(testUser.id, 1).FirstOrDefault();
                        if (testAccount == null)
                        {
                            return RedirectToAction("CustomerRegister");
                        }
                        else
                        {
                            ViewBag.Email = email;
                            ViewBag.Phone = phoneNumber;
                            ViewBag.Error = "This Email and Phone Number is alredy has an account. Please try another";
                            return View("CustomerEmailRegister");
                        }

                    }

                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult ShopOwnerEmailRegister()
        {
            HttpContext.Session.SetInt32(SessionKeys.RoleID.ToString(), 2);
            return View();
        }
        public IActionResult ShopOwnerEmailRegisterCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email,
                                                        [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    HttpContext.Session.SetString(SessionKeys.Email.ToString(), email);
                    HttpContext.Session.SetString(SessionKeys.PhoneNumber.ToString(), phoneNumber);
                    User phoneUser = context.Users.GetValidUserDataByPhone(phoneNumber).FirstOrDefault();
                    User emailUser = context.Users.GetValidUserDataByEmail(email).FirstOrDefault();
                    if (phoneUser != null && emailUser == null)
                    {
                        ViewBag.Email = email;
                        ViewBag.Phone = phoneNumber;
                        if (phoneUser != null)
                        {
                            ViewBag.ErrorPhone = "This Phone Number has already been register";
                        }
                        return View("ShopOwnerEmailRegister");

                    }
                    else
                    {
                        if (phoneUser == null && emailUser != null)
                        {
                            ViewBag.Email = email;
                            ViewBag.Phone = phoneNumber;
                            if (emailUser != null)
                            {
                                ViewBag.ErrorEmail = "This Email has already been register";
                            }
                            return View("ShopOwnerEmailRegister");
                        }
                    }
                    User? testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                    if (testUser == null)
                    {
                        return RedirectToAction("UserInformationRegister");
                    }
                    else
                    {
                        Account testAccount = context.Accounts.GetValidAccountByUserId(testUser.id, 2).FirstOrDefault();
                        if (testAccount == null)
                        {
                            return RedirectToAction("ShopOwnerRegister");
                        }
                        else
                        {
                            ViewBag.Email = email;
                            ViewBag.Phone = phoneNumber;
                            ViewBag.Error = "This Email and Phone Number is alredy has an account. Please try another";
                            return View("ShopOwnerEmailRegister");
                        }

                    }

                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }
        public IActionResult CustomerRegister()
        {
            String email = HttpContext.Session.GetString(SessionKeys.Email.ToString());
            String phoneNumber = HttpContext.Session.GetString(SessionKeys.PhoneNumber.ToString());
            if (email != null && phoneNumber != null)
            {
                return View();
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult CustomerRegisterCheck([MaxLength(100)] String username,
                                               [DataType(DataType.Password), MaxLength(100)] String password,
                                               [DataType(DataType.Password), MaxLength(100)] String passwordConfirm)
        {

            String email = HttpContext.Session.GetString(SessionKeys.Email.ToString());
            String phoneNumber = HttpContext.Session.GetString(SessionKeys.PhoneNumber.ToString());
            ViewBag.Email = email;
            ViewBag.Phone = phoneNumber;
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    User testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                    Account testAcc1 = context.Accounts.GetValidShopOwnerAccount(username, 1).FirstOrDefault();
                    Account testAcc2 = context.Accounts.GetValidShopOwnerAccount(username, 2).FirstOrDefault();
                    if (testAcc1 == null && password == passwordConfirm && testAcc2 == null)
                    {
                        Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = 1, externalId = null, externalType = null };
                        context.Add(newAccount);
                        context.SaveChanges();
                        HttpContext.Session.Remove(SessionKeys.Email.ToString());
                        HttpContext.Session.Remove(SessionKeys.PhoneNumber.ToString());
                        return RedirectToAction("RegisterSucceed");
                    }
                    else
                    {
                        ViewBag.Password = password;
                        ViewBag.PasswordConfirm = passwordConfirm;
                        ViewBag.Username = username;
                        if (testAcc1 != null)
                        {
                            if (testAcc1.username == username)
                            {
                                ViewBag.ErrorUsername = "This UserName is already been used.";
                            }
                        }
                        if (testAcc2 != null)
                        {
                            if (testAcc2.username == username)
                            {
                                ViewBag.ErrorUsername = "This UserName is already been used.";
                            }
                        }
                        if (passwordConfirm != password)
                        {
                            ViewBag.ErrorPasswordConfirm = "Password and PasswordConfirm does not match";
                        }

                        return View("CustomerRegister");
                    }
                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult ShopOwnerRegister()
        {
            String email = HttpContext.Session.GetString(SessionKeys.Email.ToString());
            String phoneNumber = HttpContext.Session.GetString(SessionKeys.PhoneNumber.ToString());
            if (email != null && phoneNumber != null)
            {
                return View();
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult ShopOwnerRegisterCheck([MaxLength(100)] String username,
                                               [DataType(DataType.Password), MaxLength(100)] String password,
                                               [DataType(DataType.Password), MaxLength(100)] String passwordConfirm)
        {

            String email = HttpContext.Session.GetString(SessionKeys.Email.ToString());
            String phoneNumber = HttpContext.Session.GetString(SessionKeys.PhoneNumber.ToString());
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    User testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                    Account testAcc1 = context.Accounts.GetValidShopOwnerAccount(username, 1).FirstOrDefault();
                    Account testAcc2 = context.Accounts.GetValidShopOwnerAccount(username, 2).FirstOrDefault();
                    if (testAcc1 == null && password == passwordConfirm && testAcc2 == null)
                    {
                        Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = 2, externalId = null, externalType = null };
                        context.Add(newAccount);
                        context.SaveChanges();
                        HttpContext.Session.Remove(SessionKeys.Email.ToString());
                        HttpContext.Session.Remove(SessionKeys.PhoneNumber.ToString());
                        return RedirectToAction("RegisterSucceed");
                    }
                    else
                    {
                        ViewBag.Password = password;
                        ViewBag.PasswordConfirm = passwordConfirm;
                        ViewBag.Username = username;
                        if (testAcc1 != null)
                        {
                            if (testAcc1.username == username)
                            {
                                ViewBag.ErrorUsername = "This UserName is already been used.";
                            }
                        }
                        if (testAcc2 != null)
                        {
                            if (testAcc2.username == username)
                            {
                                ViewBag.ErrorUsername = "This UserName is already been used.";
                            }
                        }
                        if (passwordConfirm != password)
                        {
                            ViewBag.ErrorPasswordConfirm = "Password and PasswordConfirm does not match";
                        }

                        return View("ShopOwnerRegister");
                    }
                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult UserInformationRegister()
        {
            String email = HttpContext.Session.GetString(SessionKeys.Email.ToString());
            String phoneNumber = HttpContext.Session.GetString(SessionKeys.PhoneNumber.ToString());
            if (email != null && phoneNumber != null)
            {
                return View();
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }

        }
        public IActionResult UserInformationRegisterCheck([MaxLength(100)] String displayName,
                                                          [DataType(DataType.Date), MaxLength(20)] String birthday,
                                                          [MaxLength(15)] String gender,
                                                          [MaxLength(100)] String firstName,
                                                          [MaxLength(100)] String lastName,
                                                          [MaxLength(100)] String province,
                                                          [MaxLength(100)] String district,
                                                          [MaxLength(100)] String commune,
                                                          [MaxLength(100)] String address,
                                                          [MaxLength(100)] String username,
                                                          [DataType(DataType.Password), MaxLength(100)] String password,
                                                          [DataType(DataType.Password), MaxLength(100)] String passwordConfirm)
        {
            
            String email = HttpContext.Session.GetString(SessionKeys.Email.ToString());
            String phoneNumber = HttpContext.Session.GetString(SessionKeys.PhoneNumber.ToString());
            int? id = HttpContext.Session.GetInt32(SessionKeys.RoleID.ToString());
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    User newUser = new User { email = email, phoneNumber = phoneNumber, displayName = displayName, birthday = birthday, gender = gender, firstName = firstName, lastName = lastName, province = province, district = district, commune = commune, address = address };
                    context.Add(newUser);
                    context.SaveChanges();

                        User testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                        Account testAcc1 = context.Accounts.GetValidShopOwnerAccount(username, 1).FirstOrDefault();
                        Account testAcc2 = context.Accounts.GetValidShopOwnerAccount(username, 2).FirstOrDefault();
                        if (testAcc1 == null && password == passwordConfirm && testAcc2 == null)
                        {
                            Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = id.Value, externalId = null, externalType = null };
                            context.Add(newAccount);
                            context.SaveChanges();
                            HttpContext.Session.Remove(SessionKeys.Email.ToString());
                            HttpContext.Session.Remove(SessionKeys.PhoneNumber.ToString());
                        return RedirectToAction("RegisterSucceed");
                        }
                        else
                        {
                            ViewBag.DisplayName = displayName;
                            ViewBag.FirstName = firstName; 
                            ViewBag.LastName = lastName;
                            ViewBag.Address = address;
                            ViewBag.Gender = gender;
                            ViewBag.Birthday = birthday;
                            ViewBag.Password = password;                           
                            ViewBag.PasswordConfirm = passwordConfirm;
                            ViewBag.Username = username;
                            if (testAcc1 != null)
                            {
                                if (testAcc1.username == username)
                                {
                                    ViewBag.ErrorUsername = "This UserName is already been used.";
                                }
                            }
                            if (testAcc2 != null)
                            {
                                if (testAcc2.username == username)
                                {
                                    ViewBag.ErrorUsername = "This UserName is already been used.";
                                }
                            }
                            if (passwordConfirm != password)
                            {
                                ViewBag.ErrorPasswordConfirm = "Password and PasswordConfirm does not match";
                            }

                            return View("UserInformationRegister");
                    }

                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult RegisterSucceed()
        {
            ViewBag.Id = HttpContext.Session.GetInt32(SessionKeys.RoleID.ToString());
            HttpContext.Session.Clear();
            return View();
        }


    }
}
