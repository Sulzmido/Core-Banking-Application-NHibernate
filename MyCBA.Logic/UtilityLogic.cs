using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class UtilityLogic
    {
        public void SendMail(string from, string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.From = new MailAddress(from);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["mailAccount"],  // replace with valid value
                    Password = ConfigurationManager.AppSettings["mailPassword"]  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);

            }
        }

        public string GetRandomPassword()
        {
            Random rand = new Random();
            int N = rand.Next(6, 10);   //6 to 10 characters of password
            char[] passwordChar = new char[N];
            int alphabetCount = N - 4; int numberCount = 2; int symbolCount = 2;

            char[] alphabets = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] specialCharacters = new char[] { '.', ',', ';', '/', '!', '@', '#', '$', '%', '^', '&', '*', '|' };

            //getting the alphabets
            for (int i = 0; i < alphabetCount; i++)
            {
                int index = rand.Next(0, alphabets.Count() - 1);
                var myChar = alphabets[index];
                //choose upper or lower case randomly
                int toUpper = rand.Next(0, 1);
                if (toUpper == 1)
                    myChar = myChar.ToString().ToUpper()[0];    //converts the character to upper case
                passwordChar[i] = myChar;
            }

            //getting the numbers
            int charPosition = alphabetCount;
            for (int i = 0; i < numberCount; i++)
            {
                int index = rand.Next(0, numbers.Count() - 1);
                var n = numbers[index];
                passwordChar[charPosition] = n;
                charPosition++;
            }

            //getting the special characters

            charPosition = alphabetCount + numberCount;
            for (int i = 0; i < symbolCount; i++)
            {
                int index = rand.Next(0, specialCharacters.Count() - 1);
                var symb = specialCharacters[index];
                passwordChar[charPosition] = symb;
                charPosition++;
            }

            string password = new string(passwordChar);
            return password;

            //Membership.GeneratePassword(12, 6);   this will create a random password
        }//end randomPass

        public static void LogMessage(String msg)
        {

            #region LogMessage to File
            System.Diagnostics.Trace.TraceInformation(msg);
            using (StreamWriter logWriter = new StreamWriter(@"C:\Users\Sulaiman Ahmed\Desktop\LogFiles\CBA_MessageLogs.txt", true))
            {
                logWriter.WriteLine(msg + " " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt") + Environment.NewLine);
            }
            #endregion
        }

        public static void LogError(String errorMsg)
        {

            #region LogMessage to File
            System.Diagnostics.Trace.TraceInformation(errorMsg);
            using (StreamWriter logWriter = new StreamWriter(@"C:\Users\Sulaiman Ahmed\Desktop\LogFiles\CBA_ErrorLogs.txt", true))
            {
                logWriter.WriteLine(errorMsg + " " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt") + Environment.NewLine);
            }
            #endregion
        }
    }
}
