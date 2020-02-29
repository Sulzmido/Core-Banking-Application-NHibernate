using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class UserLogic
    {
        UserRepository userRepo = new UserRepository();
        public bool IsUniqueUsername(string username)
        {
            if (userRepo.GetByUsername(username) == null)
            {
                return true;
            }
            return false;
        }
        public bool IsUniqueEmail(string email)
        {
            if (userRepo.GetByEmail(email) == null)
            {
                return true;
            }
            return false;
        }

        public bool IsUserEmailConfirmed(int userId)
        {
            var user = userRepo.GetById(userId);
            return user.EmailConfirmed;
        }

        public bool IsEmailConfirmed(string email)
        {
            var users = userRepo.GetAllUsersByEmail(email);
            //return users.Any(u => u.EmailConfirmed == true);
            foreach (var user in users)
            {
                if (user.EmailConfirmed)
                {
                    return true;
                }
            }
            return false;
        }

        public User FindConfirmedUser(string email)
        {
            var users = userRepo.GetAllUsersByEmail(email);
            //users.Find(u => u.EmailConfirmed == true);
            //users.Where(u => u.EmailConfirmed == true).FirstOrDefault();
            foreach(var user in users)
            {
                if (user.EmailConfirmed)
                {
                    return user;
                }
            }
            return null;
        }

        public void SendPasswordToUser(string fullName, string toMail, string username, string password)
        {
            var bodyFormat = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p> <h2>Thanks and have a nice day</h2>";
            string msgBody = string.Format(bodyFormat, "Imagine Nation", "xyz@gmail.com", "Dear " + fullName + ", an account has been created for you in our Banking application. Your username is \"" + username + "\" and your password is: \"" + password + "\". Please keep safely these details as they will be required of you to acess the application");
            new UtilityLogic().SendMail("xyz@gmail.com", toMail, "Your Password", msgBody);
        }

        public void SendEmailConfirmationTokenToUser(string callbackUrl, string toMail)
        {
            var bodyFormat = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            string msgBody = string.Format(bodyFormat, "Imagine Nation", "xyz@gmail.com", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            new UtilityLogic().SendMail("xyz@gmail.com", toMail, "Email Confirmation" , msgBody);
        }

        public void SendPasswordResetTokenToUser(string callbackUrl, string toMail)
        {
            var bodyFormat = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            string msgBody = string.Format(bodyFormat, "Imagine Nation", "xyz@gmail.com", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            new UtilityLogic().SendMail("xyz@gmail.com", toMail, "Reset Password", msgBody);
        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return AreHashesEqual(buffer3, buffer4);
        }

        private static bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            int _minHashLength = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < _minHashLength; i++)
                xor |= firstHash[i] ^ secondHash[i];
            //return 0 == xor;
            return xor == 0;
        }

        public void SignInUser(User user, bool rememberMe)
        {

        }
        public User FindUser(string username, string enteredPassword)
        {
            var user = userRepo.GetByUsername(username);
            //verifying password
            if (user == null)
            {
                return null;
            }
            if (VerifyHashedPassword(user.PasswordHash, enteredPassword))
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
