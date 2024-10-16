using System.Security.Cryptography;

namespace E_Retalling_Portal.Services
{
	public static class OTPService
	{
		public static string GenerateOTP(int length = 6)
		{
			byte[] randomNumber = new byte[length];

			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
			}

			var otp = "";
			foreach (var byteValue in randomNumber)
			{
				otp += (byteValue % 10).ToString();
			}

			return otp;
		}

		public static string MaskEmail(string email)
		{

			var parts = email.Split('@');
			var username = parts[0];
			var domain = parts[1];

			var maskedUsername = username.Length > 3 ? new string('*', username.Length - 3) + username.Substring(username.Length - 3) : username;

			return maskedUsername + "@" + domain;
		}
	}
}
