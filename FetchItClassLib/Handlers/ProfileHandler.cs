﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// This class handles anything that has to do with profiles. It implements the singleton pattern to prevent
    /// multiple people beeing logged in at the same time from the same program.
    /// </summary>
    public class ProfileHandler
    {

        enum ProfileStatus
        {
            Inactive = 2,
            Suspended,
            Disabled,
            Active,
            Deleted,
        }

        enum ProfileLevel
        {
            User = 0,
            Administartor = 9001,
        }

        // TODO: Should we have a (singleton) class to hold the profile while the user is logged in?
        private static ProfileModel _currentLoggedInProfile;
        private static ProfileHandler _handler;

        /// <summary>
        /// Provides access to the currently logged in profile.
        /// </summary>
        public ProfileModel CurrentLoggedInProfile
        {
            get { return _currentLoggedInProfile; }
            private set { _currentLoggedInProfile = value; }
        }


        private ProfileHandler()
        {
            
        }

        public static ProfileHandler CreateInstance()
        {
            if (_handler == null)
            {
                _handler = new ProfileHandler();
            }
            return _handler;
        }

        /// <summary>
        /// This method will give you a list of all profiles in the database
        /// </summary>
        /// <returns>List of all profiles</returns>
        public List<ProfileModel> GetAllProfiles()
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
        public void AddNewProfile(string name, string address, string phone, string mobile, string password, string email)
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
                FK_ProfileStatusId = (int)ProfileStatus.Inactive
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
        /// This method generates the activation link, and sends it by email to the user supplied email.
        /// </summary>
        /// <param name="name">Profile name</param>
        /// <param name="activation">Activation id</param>
        /// <param name="email">Email that will recieve the activation link</param>
        private void SendActivationLink(string name, string activation, string email)
        {
            var url = "http://urlOfSite.com";
            var profileToActivate = name;
            var profileactivationId = activation;
            var profileEmail = email;

            url += "?activate=" + profileToActivate;
            url += "&id=" + profileactivationId;

            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("zibat.fetchit", "password1234."),
                    EnableSsl = true
                };


                mail.From = new MailAddress("zibat.fetchit@gmail.com");
                mail.To.Add(profileEmail);
                mail.Subject = "Activation email";
                mail.Body = url;


                smtpServer.Send(mail);
            }
            catch (Exception e)
            {
                throw new EmailFailed(e.Message);
            }
            // TODO: Add table columm, to contain activation string.
        }

        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to delete a profile.
        /// And if so, change the status of the selected profile to deleted. Nothing is actually removed from the database.
        /// If modifying the profile status fails, it will throw a <see cref="ProfileUpdate"/> exception.
        /// </summary>
        /// <param name="profileIdToDelete">Profile id of the user you wish to delete.</param>
        /// <returns>True if the operation succeded</returns>
        public bool DeleteProfile(int profileIdToDelete)
        {
            if (profileIdToDelete > 0)
            {
                if (CurrentLoggedInProfile != null && CurrentLoggedInProfile.ProfileId != profileIdToDelete)
                {
                    if (CurrentLoggedInProfile.FK_ProfileLevelId >= (int)ProfileLevel.Administartor) // check if the user has admin rights.
                    {
                        try
                        {
                            using (var dbConn = new DbConn())
                            {
                                foreach (var profile in dbConn.ProfileModels.Where(profile => profile.ProfileId == profileIdToDelete))
                                {
                                    profile.FK_ProfileStatusId = (int)ProfileStatus.Deleted;
                                }
                                dbConn.SaveChanges();
                            }
                            return true;
                        }
                        catch (Exception)
                        {
                            throw new ProfileUpdate("Failed to delete the profile.");
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This will return true if the login was a success.
        /// If someone is allready logged in. It will return false.
        /// Otherwise it will throw a <see cref="FailedLogIn"/> exception.
        /// </summary>
        /// <param name="profileName">The name of the profile</param>
        /// <param name="password">The password of the profile</param>
        /// <returns>The requested profile</returns>
        public bool LogIn(string profileName, string password)
        {
            if (CurrentLoggedInProfile == null)
            {
                using (var dbconn = new DbConn())
                {
                    var selectedProfile =
                        dbconn.ProfileModels.Where(
                        profile =>
                            profile.ProfileName == profileName).ToList();

                    if (selectedProfile.Count == 1)
                    {
                        if (selectedProfile[0].FK_ProfileStatusId == (int)ProfileStatus.Deleted)
                        {
                            throw new FailedLogIn("The account " + selectedProfile[0].ProfileName + " has been deleted.");
                        }

                        if (selectedProfile[0].FK_ProfileStatusId != (int)ProfileStatus.Active)
                        {
                            throw new FailedLogIn("Account status: " + selectedProfile[0].ProfileStatus.Status);
                        }

                        var hashedPwd = HashPassword(password, selectedProfile[0].ProfilePasswordSalt);
                        if (selectedProfile[0].ProfilePassword == hashedPwd)
                        {
                            CurrentLoggedInProfile = selectedProfile[0];
                            return true;
                        }
                    }
                    throw new FailedLogIn("Wrong username or password");
                }
            }
            return false;
        }

        /// <summary>
        /// Will set the <see cref="CurrentLoggedInProfile"/> to null, if anyone is allready logged in.
        /// Will throw <see cref="FailedLogOut()"/> exception if no one was logged in.
        /// </summary>
        public void LogOut()
        {
            if (CurrentLoggedInProfile == null)
            {
                throw new FailedLogOut("No one was logged in");
            }
            CurrentLoggedInProfile = null;
        }

        /// <summary>
        /// This will generate a cryptography grade random string
        /// </summary>
        /// <returns>Random string</returns>
        private string GenerateSalt()
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
        private string HashPassword(string password, string salt)
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

    public class FailedLogOut : Exception
    {
        public FailedLogOut()
        {
            
        }
        public FailedLogOut(string message): base(message)
        {

        }
        public FailedLogOut(string message, Exception inner): base(message, inner)
        {

        }
    }

    public class ProfileUpdate : Exception
    {
        // TODO: Log the event?

        public ProfileUpdate()
        {
            
        }

        public ProfileUpdate(string message): base(message)
        {
            
        }

        public ProfileUpdate(string message, Exception inner)
            : base(message, inner)
        {
            
        }
    }

    public class EmailFailed : Exception
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
                catch (WebException)
                {
                    // this one is offline
                }

                try
                {
                    return client.DownloadString("http://wtfismyip.com/text");
                }
                catch (WebException)
                {
                    // offline...
                }

                try
                {
                    return client.DownloadString("http://ip.telize.com/");
                }
                catch (WebException)
                {
                    // offline too...
                }

                // if we got here, all the websites are down, which is unlikely
                return "No ip found";
            }
        }

    }

}
