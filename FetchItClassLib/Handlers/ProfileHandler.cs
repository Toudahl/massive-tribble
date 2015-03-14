using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using FetchItClassLib.Persistence.EF;

namespace FetchItClassLib.Handlers
{
    // Author: Morten Toudahl
    /// <summary>
    /// This class handles anything that has to do with profiles.
    /// </summary>
    static public class ProfileHandler
    {
        /// <summary>
        /// This method will give you a list of all profiles in the database
        /// </summary>
        /// <returns>List of all profiles</returns>
        static public List<ProfileModel> GetAllProfiles()
        {
            using (var dbConn = new DbConn())
            {
                return dbConn.ProfileModels.ToList();
            }
        }

        /// <summary>
        /// This method adds a new user to the profile. By default, it will be inactive
        /// </summary>
        /// <param name="name">Name of profile. Not case sensitive, but will display as entered</param>
        /// <param name="address">The address of the profile</param>
        /// <param name="phone">Phone number of the profile</param>
        /// <param name="mobile">Mobile number of the profile</param>
        /// <param name="password">Password of the profile. Case sensitive. Will be hashed in the database</param>
        /// <param name="email">contact email.</param>
        static public void AddNewProfile(string name, string address, string phone, string mobile, string password, string email)
        {
            var salt = GenerateSalt();
            var activationId = GenerateSalt();
            var newProfile = new ProfileModel
            {
                ProfileName = name,
                ProfileAddress = address,
                ProfilePhone = phone,
                ProfileMobile = mobile,
                ProfileEmail = email,
                ProfilePasswordSalt = salt,
                ProfilePassword = HashPassword(password, salt),
                FK_ProfileStatusId = 2
            };

                using (var dbConn = new DbConn())
                {
                    try
                    {
                        dbConn.ProfileModels.Add(newProfile);
                        dbConn.SaveChanges();
                        SendActivationLink(name, activationId, email);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    
                }
        }

        /// <summary>
        /// This method generated the activation link, and sends it by email to the user supplied email.
        /// </summary>
        /// <param name="name">Profile name</param>
        /// <param name="activation">Activation id</param>
        /// <param name="email">Email that will recieve the activation link</param>
        private static void SendActivationLink(string name, string activation, string email)
        {
            string url = "http://urlOfSite.com";
            string profileToActivate = name;
            string profileactivationId = activation;
            string profileEmail = email;

            url += "?activate=" + profileToActivate;
            url += "&id=" + profileactivationId;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("zibat.fetchit@gmail.com");
                mail.To.Add(profileEmail);
                mail.Subject = "Activation email";
                mail.Body = url;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("zibat.fetchit", "password1234.");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                throw new EmailFailed(e.Message);
            }
            // TODO: Make the email service work. Add table field, to contain activation string.
        }

        /// <summary>
        /// This will return a single profile model, to represent the logged in user.
        /// </summary>
        /// <param name="profileName">The name of the profile</param>
        /// <param name="password">The password of the profile</param>
        /// <returns>The requested profile</returns>
        private static ProfileModel LogIn(string profileName, string password)
        {
            using (var dbconn = new DbConn())
            {
                var selectedProfile =
                    dbconn.ProfileModels.Where(
                    profile =>
                        profile.ProfileName == profileName).ToList();

                if (selectedProfile.Count == 1)
                {
                    if (selectedProfile[0].FK_ProfileStatusId != 5)
                    {
                        throw new FailedLogIn("Account status: " + selectedProfile[0].ProfileStatus.Status);
                    }

                    var hashedPwd = HashPassword(password, selectedProfile[0].ProfilePasswordSalt);
                    if (selectedProfile[0].ProfilePassword == hashedPwd)
                    {
                        return selectedProfile[0];
                    }
                }
                throw new FailedLogIn("Wrong username or password");
            }
        }


        /// <summary>
        /// This will generate a cryptography grade random string
        /// </summary>
        /// <returns>Random string</returns>
        private static string GenerateSalt()
        {
            // http://stackoverflow.com/questions/7272771/encrypting-the-password-using-salt-in-c-sharp
            var bytes = new Byte[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// This method hashes the input, for use as a password.
        /// </summary>
        /// <param name="password">The string you wish to Hash</param>
        /// <param name="salt">The salt</param>
        /// <returns>The hashed result of the two inputs</returns>
        private static string HashPassword(string password, string salt)
        {
            var passwordAndSalt = password + salt;
            var pwdAsBytesArray = Encoding.ASCII.GetBytes(passwordAndSalt);
            string result = "";

            using (SHA256 sha256 = new SHA256Managed())
            {
                // https://msdn.microsoft.com/en-us/library/bb548651.aspx
                // Take the current, and the next as arguement. Concatenate them, and save in current.
                // Do it for all the occurances in the byte array, and add all of it to result
                return sha256.ComputeHash(pwdAsBytesArray).Aggregate(result, (current, next) => current + next);
            }
        }
    }

    internal class EmailFailed : Exception
    {
        // TODO: Log the event

        public EmailFailed()
        {
            
        }

        public EmailFailed(string message): base(message)
        {
            
        }

        public EmailFailed(string message, Exception inner): base(message,inner)
        {
            
        }
    }

    public class FailedLogIn : Exception
    {
        
        static private string _externalIp = getExternalIP();

        // TODO: Do some actual logging.

        public FailedLogIn(string message)
            : base(message + " Logged Ip: " + _externalIp)
        {

        }

        public FailedLogIn(string message, Exception inner)
            : base(message + " Logged Ip: " + _externalIp, inner)
        {

        }

        private static string getExternalIP()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadString("http://canihazip.com/s");
                }
                catch (WebException e)
                {
                    // this one is offline
                }

                try
                {
                    return client.DownloadString("http://wtfismyip.com/text");
                }
                catch (WebException e)
                {
                    // offline...
                }

                try
                {
                    return client.DownloadString("http://ip.telize.com/");
                }
                catch (WebException e)
                {
                    // offline too...
                }

                // if we got here, all the websites are down, which is unlikely
                return "No ip found";
            }
        }

    }

}
