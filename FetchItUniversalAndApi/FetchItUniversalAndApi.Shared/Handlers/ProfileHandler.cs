using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;
using Newtonsoft.Json;

namespace FetchItUniversalAndApi.Handlers
{
    // Author: Morten Toudahl
    class ProfileHandler: IDelete, ICreate, ISuspend, IDisable, IUpdate, ISearch
    {
        #region Enums
        /// <summary>
        /// The values of this enum corrosponds to the id's in the database.
        /// </summary>
        public enum ProfileStatus
        {
            Deleted = 1,
            Suspended = 2,
            Disabled = 3,
            Active = 4,
            Unactivated = 5,
        }

        /// <summary>
        /// The values of this enum corrosponds to the id's in the database.
        /// </summary>
        public enum ProfileLevel
        {
            User = 1,
            Administrator = 9001,
        }

        /// <summary>
        /// The values of this enum corrosponds to the id's in the database.
        /// </summary>
        public enum ProfileVerificationType
        {
            IRL = 1,
            Passport = 2,
            NotVerified = 3,
        }
        #endregion

        #region Fields and properties.

        private const string Apiurl = "http://fetchit.mortentoudahl.dk/api/ProfileModels";
        private ProfileModel _currentLoggedInProfile;
        private ProfileModel _selectedProfile;
        private IEnumerable<ProfileModel> _allProfiles;
        private static ProfileHandler _handler;
        private static Object _lockObject = new object();

        /// <summary>
        /// This contains the currently logged in profile.
        /// </summary>
        public ProfileModel CurrentLoggedInProfile
        {
            get { return _currentLoggedInProfile; }
            private set { _currentLoggedInProfile = value; }
        }

        /// <summary>
        /// Contains the currently selected profile. IE, could be used to hold a profile to delete.
        /// But is it nessesary.
        /// </summary>
        public ProfileModel SelectedProfile
        {
            get { return _selectedProfile; }
            set { _selectedProfile = value; }
        }

        /// <summary>
        /// Contains all profiles.
        /// </summary>
        public IEnumerable<ProfileModel> AllProfiles
        {
            get { return _allProfiles; }
            set { _allProfiles = value; }
        }

        #endregion

        #region Singleton section
        /// <summary>
        /// Private constructor, to implement the singleton pattern
        /// </summary>
        private ProfileHandler()
        {
            
        }

        /// <summary>
        /// Get an instance of the <see cref="ProfileHandler"/>
        /// This singleton is Thread safe
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
        #endregion

        #region Delete method

        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to delete a profile.
        /// And if so, change the status of the selected profile to deleted. Nothing is actually removed from the database.
        /// If modifying the profile status fails, it will throw an exception.
        /// </summary>
        /// <param name="obj">The profile to delete</param>
        public void Delete(object obj)
        {
            if (CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileLevel.Administrator)
            {
                if (obj is ProfileModel)
                {
                    var profileToDelete = obj as ProfileModel;
                    if (profileToDelete == CurrentLoggedInProfile)
                    {
                        ErrorHandler.WrongTargetProfile("delete");
                    }
                    else
                    {
                        ChangeStatus(profileToDelete, ProfileStatus.Deleted);
                    }
                }
                else
                {
                    ErrorHandler.WrongModelError(obj, new ProfileModel());
                }
            }
            else
            {
                ErrorHandler.WrongProfileLevel((ProfileLevel)CurrentLoggedInProfile.FK_ProfileLevel, "delete");
            }
        }
        #endregion

        #region Create method
        /// <summary>
        /// This method adds a new user to the profile. By default, it will be an Unactivated User that has not been verified.
        /// </summary>
        /// <param name="obj">Pass in a ProfileModel with the state that you wish to create the profile in.</param>
        public async void Create(object obj)
        {
            if (obj is ProfileModel)
            {
                var newProfile = obj as ProfileModel;
                newProfile.FK_ProfileLevel = (int)ProfileLevel.User;
                newProfile.FK_ProfileStatus = (int) ProfileStatus.Active;
                newProfile.ProfileIsVerified = false;
                newProfile.FK_ProfileVerificationType = (int) ProfileVerificationType.NotVerified;
                newProfile.ProfileCanReport = 1;

                //TODO create method to generate a salt, and hash the users password with it.
                newProfile.ProfilePasswordSalt = 12345678;
                //newProfile.ProfilePassword = HashPassword(newProfile.ProfilePassword, newProfile.ProfilePasswordSalt);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    await client.PostAsJsonAsync(Apiurl, newProfile);
                }
            }
            else
            {
                ErrorHandler.WrongModelError(obj, new ProfileModel());
            }
        }
        #endregion

        #region Suspend method
        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to suspend a profile.
        /// And if so, change the status of the selected profile to suspended.
        /// If modifying the profile status fails, it will throw a <see cref="ProfileUpdate"/> exception.
        /// </summary>
        /// <param name="obj">The profile to suspend</param>
        public void Suspend(object obj)
        {
            if (CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileLevel.Administrator)
            {
                if (obj is ProfileModel)
                {
                    var profileToSuspend = obj as ProfileModel;
                    if (profileToSuspend == CurrentLoggedInProfile)
                    {
                        ErrorHandler.WrongTargetProfile("suspend");
                    }
                    else
                    {
                        ChangeStatus(profileToSuspend, ProfileStatus.Suspended);
                    }
                }
                else
                {
                    ErrorHandler.WrongModelError(obj, new ProfileModel());
                }
            }
            else
            {
                ErrorHandler.WrongProfileLevel((ProfileLevel)CurrentLoggedInProfile.FK_ProfileLevel,"suspend");
            }
        }
        #endregion

        #region Disable method
        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to disable a profile.
        /// And if so, change the status of the selected profile to suspended.
        /// If modifying the profile status fails, it will throw a <see cref="ProfileUpdate"/> exception.
        /// </summary>
        /// <param name="obj">The profile to disable</param>
        public void Disable(object obj)
        {
            if (CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileLevel.User)
            {
                if (obj is ProfileModel)
                {
                    var profileToDisable = obj as ProfileModel;
                    if (profileToDisable != CurrentLoggedInProfile)
                    {
                        ErrorHandler.WrongTargetProfile("suspend");
                    }
                    else
                    {
                        ChangeStatus(CurrentLoggedInProfile, ProfileStatus.Disabled);
                    }

                }
                else
                {
                    ErrorHandler.WrongModelError(obj, new ProfileModel());
                }
            }
            else
            {
                ErrorHandler.WrongProfileLevel((ProfileLevel)CurrentLoggedInProfile.FK_ProfileLevel, "disable");
            }
        }
        #endregion

        #region Update method
        /// <summary>
        /// This method will take the ProfileModel it is supplied, and update it on the server
        /// Do NOT change the ID of a profile. TODO: make the webapi prevent change of name.
        /// </summary>
        /// <param name="obj">A ProfileModel</param>
        public async void Update(object obj)
        {
            if (obj is ProfileModel)
            {
                var profil = obj as ProfileModel;
                var url = Apiurl + "/" + profil.ProfileId;
                using (var client = new HttpClient())
                {
                    try
                    {
                        await client.PutAsJsonAsync(url, profil);
                    }
                    catch (Exception)
                    {
                        ErrorHandler.NoResponseFromApi();
                    }
                }
            }
            else
            {
                ErrorHandler.WrongModelError(obj, new ProfileModel());
            }
            
        }
        #endregion

        #region Search method
        /// <summary>
        /// Returns a list of ProfileModels, that matches the exact content of either, ProfileId, ProfileName or ProfileEmail.
        /// The webapi should be updated to make the search at the api, and return the result.
        /// </summary>
        /// <param name="obj">Object with the information to search for.</param>
        /// <returns>A list of objects that matches the search criteria</returns>
        public IEnumerable<object> Search(object obj)
        {
            if (obj is ProfileModel)
            {
                var needle = obj as ProfileModel;
                IEnumerable<ProfileModel> haystack;
                using (var client = new HttpClient())
                {
                    haystack = Task.Run(
                         async () =>
                             JsonConvert.DeserializeObject<IEnumerable<ProfileModel>>(await client.GetStringAsync(Apiurl))).Result;
                }

                if (needle.ProfileId != 0)
                {
                    return haystack.Where(p => p.ProfileId == needle.ProfileId);
                }
                if (needle.ProfileName != null)
                {
                    return haystack.Where(p => p.ProfileName == needle.ProfileName);
                }
                if (needle.ProfileEmail != null)
                {
                    return haystack.Where(p => p.ProfileEmail == needle.ProfileName);
                }
                return null;
            }
            else
            {
                ErrorHandler.WrongModelError(obj, new ProfileModel());
            }
            return null;
        }
        #endregion

        #region LogIn method
        /// <summary>
        /// This method will log you into a profile, if the object you pass it contains the correct information.
        /// </summary>
        /// <param name="profile">Must contain password and username</param>
        public async void LogIn(ProfileModel profile)
        {
            if (profile.ProfilePassword != null && profile.ProfileName != null)
            {
                using (var client = new HttpClient())
                {
                    // TODO update the webapi, so i dont have to request all the information like this.
                    // All the user information is up for graps each time someone logs in.
                    try
                    {
                        var result = await client.GetStringAsync(Apiurl);
                        var listOfProfiles = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<ProfileModel>>(result));
                        try
                        {
                            //TODO after making the hasing and salting work. Change this, so it uses the hashed password for the check
                            var selectedProfile =
                                listOfProfiles.FirstOrDefault(p => p.ProfileName == profile.ProfileName && p.ProfilePassword == profile.ProfilePassword);

                            if (selectedProfile.FK_ProfileStatus == (int)ProfileStatus.Active)
                            {
                                CurrentLoggedInProfile = selectedProfile;
                            }
                            else
                            {
                                ErrorHandler.WrongProfileStatus();
                            }
                        }
                        catch (Exception)
                        {
                            ErrorHandler.FailedLogIn(profile.ProfileName);
                        }

                    }
                    catch (Exception)
                    {
                        ErrorHandler.NoResponseFromApi();
                    }
                }
            }
            else
            {
                ErrorHandler.RequiredFields(new List<string>{"Username","Password"});
            }
        }
        #endregion

        #region LogOut method
        /// <summary>
        /// Sets the <see cref="CurrentLoggedInProfile"/> to null.
        /// </summary>
        public void LogOut()
        {
            CurrentLoggedInProfile = null;
        }
        #endregion

        #region 'Supporting methods'
        private async void ChangeStatus(ProfileModel profileToModify, ProfileStatus newStatus)
        {
            using (var client = new HttpClient())
            {
                var url = Apiurl + "/" + profileToModify.ProfileId;
                profileToModify.FK_ProfileStatus = (int)newStatus;
                profileToModify.ProfileStatus = null;
                profileToModify.ProfileLevel = null;
                try
                {
                    await client.PutAsJsonAsync(url, profileToModify);
                }
                catch (Exception)
                {
                    ErrorHandler.NoResponseFromApi();
                }
            }
        }

        public async void GetAllProfiles()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(Apiurl);
                AllProfiles = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<ProfileModel>>(result));
            }
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

        #endregion
    }
}