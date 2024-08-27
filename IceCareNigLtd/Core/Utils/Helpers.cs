using System;
using System.Net.Mail;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Core.Utils
{
	public class Helpers
	{
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Example validation for phone number; adjust as needed
            return phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 10;
        }
    }
}

