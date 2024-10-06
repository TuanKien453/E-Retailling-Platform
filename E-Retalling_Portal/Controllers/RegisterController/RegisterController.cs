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

namespace E_Retalling_Portal.Controllers.AccountController
{
    public class RegisterController : Controller
    {
        public IActionResult CustomerEmailRegister() {
            TempData["id"] = "1";
            return View();
        }
        public IActionResult CustomerEmailRegisterCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email, [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    TempData["emailData"] = email;
                    TempData["phoneNumberData"] = phoneNumber;
                    User phoneUser = context.Users.GetValidUserDataByPhone(phoneNumber).FirstOrDefault();
                    User emailUser = context.Users.GetValidUserDataByEmail(email).FirstOrDefault();
                    if(phoneUser != null && emailUser == null)
                    {
                            ViewBag.Email = email;
                            ViewBag.Phone = phoneNumber;
                            if (phoneUser != null)
                            {
                                ViewBag.ErrorPhone = "This Phone Number has already been register";
                            }
                            return View("CustomerEmailRegister");
                        
                    } else
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
                        TempData["state"] = "yes";
                        return RedirectToAction("UserInformationRegister");
                    } else
                    {
                        Account testAccount = context.Accounts.GetValidAccountByUserId(testUser.id, 1).FirstOrDefault();
                        if (testAccount == null)
                        {
                            return RedirectToAction("CustomerRegister");
                        } else
                        {
                            ViewBag.Email = email;
                            ViewBag.Phone = phoneNumber;
                            ViewBag.Error = "This Email and Phone Number is alredy has an account. Please try another";
                            return View("CustomerEmailRegister");
                        }
                        
                    }

                }
            } else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult ShopOwnerEmailRegister()
        {
            TempData["id"] = "2";
            return View();
        }
        public IActionResult ShopOwnerEmailRegisterCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email, 
                                                        [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    TempData["emailData"] = email;
                    TempData["phoneNumberData"] = phoneNumber;
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
                        TempData["state"] = "yes";
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
            if (TempData["emailData"] != null && TempData["phoneNumberData"] != null)
            {
                TempData.Keep();
                ViewBag.Email = TempData.Peek("emailData");
                ViewBag.Phone = TempData.Peek("phoneNumberData");
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
            ViewBag.Email = TempData.Peek("emailData");
            ViewBag.Phone = TempData.Peek("phoneNumberData");
            String email = TempData.Peek("emailData") as String;
            String phoneNumber = TempData.Peek("phoneNumberData") as String;
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                        User testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                        Account testAcc = context.Accounts.GetValidShopOwnerAccount(username, 1).FirstOrDefault();
                        if (testAcc == null && password == passwordConfirm)
                        {
                            Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = 1, externalId = null, externalType = null };
                            context.Add(newAccount);
                            context.SaveChanges();
                            TempData.Remove("emailData");
                            TempData.Remove("phoneNumberData");
                            TempData.Remove("id");
                            return RedirectToAction("RegisterSucceed");
                        }
                        else
                        {
                            ViewBag.Password = password;
                            ViewBag.PasswordConfirm = passwordConfirm;
                            ViewBag.Username = username;
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
            if (TempData["emailData"] != null && TempData["phoneNumberData"] != null)
            {
                TempData.Keep();
                ViewBag.Email = TempData.Peek("emailData");
                ViewBag.Phone = TempData.Peek("phoneNumberData");
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
            
            String email = TempData.Peek("emailData") as String;
            String phoneNumber = TempData.Peek("phoneNumberData") as String;
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    User testUser = context.Users.GetValidUserData(email, phoneNumber).FirstOrDefault();
                    Account testAcc = context.Accounts.GetValidShopOwnerAccount(username, 2).FirstOrDefault();
                    if (testAcc == null && password == passwordConfirm)
                    {
                        Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = 2, externalId = null, externalType = null };
                        context.Add(newAccount);
                        context.SaveChanges();
                        TempData.Remove("emailData");
                        TempData.Remove("phoneNumberData");
                        TempData.Remove("id");
                        return RedirectToAction("RegisterSucceed");
                    }
                    else
                    {
                        ViewBag.Password = password;
                        ViewBag.PasswordConfirm = passwordConfirm;
                        ViewBag.Username = username;
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
            if (TempData["emailData"] != null && TempData["phoneNumberData"] != null && TempData["state"] != null)
            {
                TempData.Keep();
                ViewBag.Email = TempData.Peek("emailData");
                ViewBag.Phone = TempData.Peek("phoneNumberData");
                return View();
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
            
        }
        public IActionResult UserInformationRegisterCheck([MaxLength(100)] String displayName, [DataType(DataType.Date), MaxLength(20)] String birthday,
                                                          [MaxLength(15)] String gender, [MaxLength(100)] String firstName, [MaxLength(100)] String lastName,
                                                          [MaxLength(100)] String address)
        {

            String email = TempData.Peek("emailData") as String;
            String phoneNumber = TempData.Peek("phoneNumberData") as String;
            string id = TempData.Peek("id") as string;
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                        User newUser = new User { email = email, phoneNumber = phoneNumber, displayName = displayName, birthday = birthday, gender = gender, firstName = firstName, lastName = lastName, address = address };
                        context.Add(newUser);
                        context.SaveChanges();
                        if (id == "1")
                        {
                        TempData.Remove("state");
                        return RedirectToAction("CustomerRegister");
                        } else
                        {
                        TempData.Remove("state");
                        return RedirectToAction("ShopOwnerRegister");
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
            return View();
        }

        
    }
}
