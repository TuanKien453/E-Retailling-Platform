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

namespace E_Retalling_Portal.Controllers.AccountController
{
    public class RegisterController : Controller
    {
        public IActionResult CustomerRegister()
        {
            return View();
        }

        public IActionResult CustomerRegisterCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email, [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber, [MaxLength(100)] String username,
                                               [DataType(DataType.Password), MaxLength(100)] String password, [DataType(DataType.Password), MaxLength(100)] String passwordConfirm)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    User testUser = context.Users.GetVaildUserData(email, phoneNumber).FirstOrDefault();
                    if (testUser == null)
                    {
                        return RedirectToAction("CustomerRegisterWithoutUser");
                    }
                    else
                    {
                        Account testAcc = context.Accounts.GetVaildShopOwnerAccount(username, 1).FirstOrDefault();
                        if (testAcc == null && password == passwordConfirm)
                        {
                            Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = 1, externalId = null, externalType = null };
                            context.Add(newAccount);
                            context.SaveChanges();
                            return RedirectToAction("RegisterSucceed");
                        }
                        else
                        {
                            ViewBag.PhoneNumber = phoneNumber;
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

                            return View("CustomerRegister");
                        }
                    }

                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult CustomerRegisterWithoutUser()
        {
            return View();
        }

        public IActionResult CustomerRegisterWithoutUserCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email, [MaxLength(100)] String username, [DataType(DataType.Password), MaxLength(100)] String password,
                                            [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber, [MaxLength(100)] String displayName, [DataType(DataType.Date), MaxLength(20)] String birthday,
                                            [MaxLength(15)] String gender, [MaxLength(100)] String firstName, [MaxLength(100)] String lastName, [DataType(DataType.Password), MaxLength(100)] String passwordConfirm,
                                            [MaxLength(100)] String address)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {

                    User testUser = context.Users.GetVaildUserData(email, phoneNumber).FirstOrDefault();
                    Account testAcc = context.Accounts.GetVaildAccount(username).FirstOrDefault();
                    if (testUser == null && testAcc == null && password == passwordConfirm)
                    {
                        User newUser = new User { email = email, phoneNumber = phoneNumber, displayName = displayName, birthday = birthday, gender = gender, firstName = firstName, lastName = lastName, address = address };
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

                        return View("CustomerRegisterWithoutUser");
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
            return View();
        }

        public IActionResult ShopOwnerRegisterCheck ([DataType(DataType.EmailAddress), MaxLength(100)] String email, [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber, [MaxLength(100)] String username,
                                               [DataType(DataType.Password), MaxLength(100)] String password, [DataType(DataType.Password), MaxLength(100)] String passwordConfirm)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {
                    User testUser = context.Users.GetVaildUserData(email, phoneNumber).FirstOrDefault();
                    if (testUser == null)
                    {
                        return RedirectToAction("ShopOwnerRegisterWithoutUser");
                    }
                    else
                    {
                        Account testAcc = context.Accounts.GetVaildShopOwnerAccount(username, 2).FirstOrDefault();
                        if (testAcc == null && password == passwordConfirm)
                        {
                            Account newAccount = new Account { userId = testUser.id, username = username, password = password, roleId = 2, externalId = null, externalType = null };
                            context.Add(newAccount);
                            context.SaveChanges();
                            return RedirectToAction("RegisterSucceed");
                        }
                        else
                        {
                            ViewBag.PhoneNumber = phoneNumber;
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
                            return View("ShopOwnerRegister");
                        }
                    }

                }
            }
            else
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        public IActionResult ShopOwnerRegisterWithoutUser()
        {
            return View();
        }

        public IActionResult ShopOwnerRegisterWithoutUserCheck([DataType(DataType.EmailAddress), MaxLength(100)] String email, [MaxLength(100)] String username, [DataType(DataType.Password), MaxLength(100)] String password,
                                            [DataType(DataType.PhoneNumber), MaxLength(15)] String phoneNumber, [MaxLength(100)] String displayName, [DataType(DataType.Date), MaxLength(20)] String birthday,
                                            [MaxLength(15)] String gender, [MaxLength(100)] String firstName, [MaxLength(100)] String lastName, [DataType(DataType.Password), MaxLength(100)] String passwordConfirm,
                                            [MaxLength(100)] String address)
        {
            if (ModelState.IsValid == true)
            {
                using (var context = new Context())
                {

                    User testUser = context.Users.GetVaildUserData(email, phoneNumber).FirstOrDefault();
                    Account testAcc = context.Accounts.GetVaildAccount(username).FirstOrDefault();
                    if (testUser == null && testAcc == null && password == passwordConfirm)
                    {
                        User newUser = new User { email = email, phoneNumber = phoneNumber, displayName = displayName, birthday = birthday, gender = gender, firstName = firstName, lastName = lastName, address = address };
                        context.Add(newUser);
                        context.SaveChanges();
                        newUser = context.Users.GetUserIdByEmail(email).FirstOrDefault();
                        Account newAccount = new Account { userId = newUser.id, username = username, password = password, roleId = 2, externalId = null, externalType = null };
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

                        return View("ShopOwnerRegisterWithoutUser");
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
