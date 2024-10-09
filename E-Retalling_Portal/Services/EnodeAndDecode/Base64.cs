using System.Security.Cryptography;
using System.Text;

namespace E_Retalling_Portal.Services.EnodeAndDecode
{
	public static class Base64
	{
		public static string EncodeToBase64(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}


		public static string DecodeFromBase64(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}


	}
}
