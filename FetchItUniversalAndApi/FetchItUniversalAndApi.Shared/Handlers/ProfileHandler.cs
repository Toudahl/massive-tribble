using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;
using Newtonsoft.Json;

namespace FetchItUniversalAndApi.Handlers
{
    class ProfileHandler: IDelete, ICreate, ISuspend, IDisable, IUpdate, ISearch
    {
        public enum ProfileStatus
        {
            Deleted = 1,
            Suspended = 2,
            Disabled = 3,
            Active = 4,
            Unactivated = 5,
        }

        public enum ProfileLevel
        {
            User = 1,
            Administartor = 9001,
        }

        public enum ProfileVerificationType
        {
            IRL = 1,
            Passport = 2,
            NotVerified = 3,
        }


        private const string Apiurl = "http://fetchit.mortentoudahl.dk/api/ProfileModels";
        private ProfileModel _currentLoggedInProfile;
        private ProfileModel _selectedProfile;
        private static ProfileHandler _handler;
        private static Object _lockObject = new object();

        public ProfileModel CurrentLoggedInProfile
        {
            get { return _currentLoggedInProfile; }
            private set { _currentLoggedInProfile = value; }
        }

        public ProfileModel SelectedProfile
        {
            get { return _selectedProfile; }
            set { _selectedProfile = value; }
        }

        private ProfileHandler()
        {
            
        }

        /// <summary>
        /// Get an instance of the <see cref="ProfileHandler"/>
        /// </summary>
        /// <returns>ProfileHandler object</returns>
        public static ProfileHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new ProfileHandler();
                }
                return _handler;
            }
        }

        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to delete a profile.
        /// And if so, change the status of the selected profile to deleted. Nothing is actually removed from the database.
        /// If modifying the profile status fails, it will throw a <see cref="ProfileUpdate"/> exception.
        /// </summary>
        /// <param name="profileIdToDelete">Profile id of the user you wish to delete.</param>
        /// <returns>True if the operation succeded. False if the user is not high enough level</returns>
        public async void Delete(object obj)
        {
            if (CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileLevel.Administartor)
            {
                if (obj is ProfileModel)
                {
                    var profileToDelete = obj as ProfileModel;

                    using (var client = new HttpClient())
                    {
                        var url = Apiurl + "/" + profileToDelete.ProfileId;
                        profileToDelete.FK_ProfileStatus = (int)ProfileStatus.Deleted;
                        profileToDelete.ProfileStatus = null;
                        profileToDelete.ProfileLevel = null;
                        try
                        {
                           await client.PutAsJsonAsync(url, profileToDelete);
                        }
                        catch (Exception)
                        {
                            //TODO failed to update the db.
                            throw new NotImplementedException();
                        }
                    }
                }
                else
                {
                    //TODO throw wrong object exception
                    throw new NotImplementedException();
                }
            }
            else
            {
                // TODO throw custom profile level exception
                throw new NotImplementedException();
            }
        }


        public async void Create(object obj)
        {
            if (obj is ProfileModel)
            {
                var newProfile = obj as ProfileModel;
                newProfile.FK_ProfileLevel = (int)ProfileLevel.User;
                newProfile.FK_ProfileStatus = (int) ProfileStatus.Unactivated;
                newProfile.ProfileIsVerified = false;
                newProfile.FK_ProfileVerificationType = (int) ProfileVerificationType.NotVerified;
                newProfile.ProfileCanReport = 1;

                //TODO create method to generate a salt, and hash the users password with it.
                newProfile.ProfilePasswordSalt = 12345678;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        await client.PostAsJsonAsync(Apiurl, newProfile);
                    }
                    catch (Exception)
                    {
                        //TODO failed to update the db.
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                //TODO throw wrong object exception
                throw new NotImplementedException();
            }
        }


        public void Suspend(object obj)
        {
            throw new NotImplementedException();
        }

        public void Disable(object obj)
        {
            throw new NotImplementedException();
        }

        public void Update(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> Search(object obj)
        {
            throw new NotImplementedException();
        }

        public async void LogIn(ProfileModel profile)
        {
            if (profile.ProfilePassword != null && profile.ProfileName != null)
            {
                using (var client = new HttpClient())
                {
                    // TODO update the webapi, so i dont have to request all the information like this.
                    // All the user information is up for graps each time someone logs in.
                    var result = await client.GetStringAsync(Apiurl);
                    var listOfProfiles = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<ProfileModel>>(result));

                    try
                    {
                        var selectedProfile =
                            listOfProfiles.FirstOrDefault(p => p.ProfileName == profile.ProfileName && p.ProfilePassword == profile.ProfilePassword);

                        if (selectedProfile.FK_ProfileStatus == (int)ProfileStatus.Active)
                        {
                            CurrentLoggedInProfile = selectedProfile;
                        }
                        else
                        {
                            //TODO throw a proper exception
                            throw new NotImplementedException();
                        }

                    }
                    catch (Exception)
                    {
                        //TODO throw a proper exception
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public void LogOut()
        {
            CurrentLoggedInProfile = null;
        }

        //#region GenerateSalt()
        ///// <summary>
        ///// This will generate a cryptographic grade random string
        ///// </summary>
        ///// <returns>Random string</returns>
        //private string GenerateSalt()
        //{
        //    // http://stackoverflow.com/questions/7272771/encrypting-the-password-using-salt-in-c-sharp
        //    var bytes = new Byte[32];
        //    using (var random = new RNGCryptoServiceProvider())
        //    {
        //        random.GetBytes(bytes);
        //        string result = "";
        //        // TODO: look into stringbuilders
        //        foreach (byte b in bytes)
        //        {
        //            result += b;
        //        }
        //        return result;
        //    }
        //}
        //#endregion

        //#region HashPassword()
        ///// <summary>
        ///// This method hashes the input, for use as a password.
        ///// </summary>
        ///// <param name="password">The string you wish to Hash</param>
        ///// <param name="salt">The salt</param>
        ///// <returns>The hashed result of the two inputs</returns>
        //private string HashPassword(string password, string salt)
        //{
        //    var passwordAndSalt = password + salt;
        //    var pwdAsBytesArray = Encoding.UTF8.GetBytes(passwordAndSalt);
        //    string result = "";
        //    var sha256 = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            
        //        // https://msdn.microsoft.com/en-us/library/bb548651.aspx
        //        // Take the current, and the next as arguement. Concatenate them, and save in current.
        //        // Do it for all the occurances in the byte array, and add all of it to result
        //        return sha256.ComputeHash(pwdAsBytesArray).Aggregate(result, (current, next) => current + next);
        //}
        //#endregion

    }
}
