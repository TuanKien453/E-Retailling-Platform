using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;
using MailKit.Security;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace E_Retalling_Portal.Controllers.UserProfile
{
    public class UserProfileController : Controller
    {
        private GHNService ghNService;

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
                Account account = context.Accounts.GetAccountByAccountId(accountId.Value).FirstOrDefault();

                User newUser = context.Users.GetUserByUserIdInAccount(account.userId).FirstOrDefault();

                User testUser = context.Users.GetValidUserData(user.email, user.phoneNumber, newUser.id).FirstOrDefault();

                if (testUser != null)
                {
                    newUser.displayName = user.displayName;
                    newUser.birthday = user.birthday;
                    newUser.gender = user.gender;
                    newUser.firstName = user.firstName;
                    newUser.lastName = user.lastName;
                    newUser.province = user.province;
                    newUser.district = user.district;
                    newUser.ward = user.ward;
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
                    newUser.phoneNumber = user.phoneNumber;
                    newUser.birthday = user.birthday;
                    newUser.gender = user.gender;
                    newUser.firstName = user.firstName;
                    newUser.lastName = user.lastName;
                    newUser.province = user.province;
                    newUser.district = user.district;
                    newUser.ward = user.ward;
                    newUser.address = user.address;
                    newUser.province = user.province;
                    newUser.district = user.district;
                    newUser.ward = user.ward;
                    if (newUser.email != user.email)
                    {
                        newUser.email = user.email;
                        HttpContext.Session.SetString(SessionKeys.UserToUpdate.ToString(), JsonConvert.SerializeObject(user));
                        return RedirectToAction("SendOTP", new { emailTo = user.email });
                    }
                    context.SaveChanges();
                    TempData["UpdateMessage"] = "Updated Successfully!";
                    return RedirectToAction("ViewProfile");
                }
                 
                
            }

        }

		[HttpPost]
		public IActionResult VerifyOTP(string enteredOtp)
		{
			var storedOtp = HttpContext.Session.GetString(SessionKeys.Otp.ToString());
			var expirationString = HttpContext.Session.GetString(SessionKeys.OtpExpiration.ToString());

			DateTime expirationTime;
			if (DateTime.TryParse(expirationString, out expirationTime))
			{
				ViewBag.ExpirationTime = expirationTime.ToString();
				if (DateTime.UtcNow <= expirationTime)
				{
					if (storedOtp == enteredOtp)
					{
						HttpContext.Session.Remove(SessionKeys.Otp.ToString());
						HttpContext.Session.Remove(SessionKeys.OtpExpiration.ToString());
						var userJson = HttpContext.Session.GetString(SessionKeys.UserToUpdate.ToString());
						if (userJson != null)
                        {
                            User userToUpdate = JsonConvert.DeserializeObject<User>(userJson);
                            using (var context = new Context())
                            {
                                int? accountId = (int)HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                                User newUser = context.Users.GetUserByUserIdInAccount(accountId.Value).FirstOrDefault();

                                newUser.displayName = userToUpdate.displayName;
                                newUser.email = userToUpdate.email;
                                newUser.phoneNumber = userToUpdate.phoneNumber;
                                newUser.birthday = userToUpdate.birthday;
                                newUser.gender = userToUpdate.gender;
                                newUser.firstName = userToUpdate.firstName;
                                newUser.lastName = userToUpdate.lastName;
                                newUser.address = userToUpdate.address;
                                newUser.province = userToUpdate.province;
                                newUser.district = userToUpdate.district;
                                newUser.ward = userToUpdate.ward;
                                context.SaveChanges(); 
                            }

                            TempData["UpdateMessage"] = "Updated Successfully!";
                            return RedirectToAction("ViewProfile");
                        }
					}
					else
					{
						ViewBag.OtpError = "Invalid OTP. Please try again!";
						return View("OTPReceive");
					}
				}
				else
				{
					ViewBag.OtpError = "Your OTP has expired. Please request a new one!";
					HttpContext.Session.Remove(SessionKeys.Otp.ToString());
					HttpContext.Session.Remove(SessionKeys.OtpExpiration.ToString());
					return View("OTPReceive");
				}
			}

			return View();
		}

		public IActionResult SendOTP(string emailTo)
		{
			HttpContext.Session.SetString(SessionKeys.Email.ToString(), emailTo);
			var email = new MimeMessage();

			var otp = OTPService.GenerateOTP();
			HttpContext.Session.SetString(SessionKeys.Otp.ToString(), otp);

			DateTime expirationTime = DateTime.UtcNow.AddMinutes(1);
			HttpContext.Session.SetString(SessionKeys.OtpExpiration.ToString(), expirationTime.ToString());

			email.From.Add(new MailboxAddress("E-Retailing-Portal", "quya1k48@gmail.com"));
			email.To.Add(new MailboxAddress("", emailTo));

			email.Subject = "Verify OTP To Register";
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = $@"
        <html>
        <body>
            <p>We have received a request to update information of your account. To confirm this request, please enter the OTP below:</p>
            <h2 style='color: blue;'>{otp}</h2>
            <p>This OTP will expire in 1 minutes. If you did not request a verification, you can safely ignore this email.</p>
            <p>Thank you for using our service!</p>
            <p>Best regards,<br>[E-Retailing-Portal]<br>[Contact Information: 0862003064]</p>
        </body>
        </html>"
			};

			using (var smtp = new SmtpClient())
			{
				smtp.Connect("smtp.gmail.com", 587, false);
				smtp.Authenticate("quya1k48@gmail.com", "dwuueitkzbynxhhk");
				smtp.Send(email);
				smtp.Disconnect(true);
			}

			return View("OTPReceive");
		}

	}
}

