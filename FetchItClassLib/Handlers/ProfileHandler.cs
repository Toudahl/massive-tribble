using System;
using System.Collections.Generic;
using System.Linq;
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
            using (var dbConn = new FetchItDatabaseEntities())
            {
                return dbConn.ProfileModels.ToList();
            }
        }

        static public void AddNewProfile(string name, string address, string phone, string mobile, string password, string email)
        {
            var salt = GenerateSalt();
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

                using (var dbConn = new FetchItDatabaseEntities())
                {
                    try
                    {
                        dbConn.ProfileModels.Add(newProfile);
                        dbConn.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                }
        }

        /// <summary>
        /// This will return a single profile model, to represent the logged in user.
        /// </summary>
        /// <param name="profileName">The name of the profile</param>
        /// <param name="password">The password of the profile</param>
        /// <returns>The requested profile</returns>
        private static ProfileModel LogIn(string profileName, string password)
        {
            using (var dbconn = new FetchItDatabaseEntities())
            {
                var selectedProfile =
                    dbconn.ProfileModels.Where(
                    profile =>
                        profile.ProfileName == profileName).ToList();

                if (selectedProfile.Count == 1)
                {
                    var hashedPwd = HashPassword(password, selectedProfile[0].ProfilePasswordSalt);
                    if (selectedProfile[0].ProfilePassword == hashedPwd)
                    {
                        return selectedProfile[0];
                    }
                }
                throw new WrongPasswordOrUsername("Wrong username or password");
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

    public class WrongPasswordOrUsername : Exception
    {
        public WrongPasswordOrUsername()
        {

        }

        public WrongPasswordOrUsername(string message)
            : base(message)
        {

        }

        public WrongPasswordOrUsername(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

}
