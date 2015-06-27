using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;
using Newtonsoft.Json;

namespace FetchItUniversalAndApi.Handlers
{
    // Author: Morten Toudahl
    /// <summary>
    /// This handler will take care of anything that has to do with profiles.
    /// </summary>
    class ProfileHandler : IDelete<ProfileModel>, ICreate<ProfileModel>, ISuspend<ProfileModel>, IDisable<ProfileModel>, IUpdate<ProfileModel>, ISearch<ProfileModel>
    {
        #region Events & delegates

        public delegate void LogInDelegate();
        public event LogInDelegate LogInEvent;

        public delegate void LogOutDelegate();
        public event LogOutDelegate LogOutEvent;

        public delegate void CreateProfile(bool success);
        public event CreateProfile CreationEvent;

        delegate void UpdateStatusOrProfile(bool success);
        private event UpdateStatusOrProfile UpdateEvent;

        #endregion

        #region Enums
        /// <summary>
        /// The values of this enum corrosponds to the id's in the database.
        /// </summary>
        public enum ProfileStatus
        {
            Delete = 1,
            Suspend = 2,
            Disable = 3,
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
        private ProfileModel _currentLoggedInProfile;
        private ProfileModel _selectedProfile;
        private IEnumerable<ProfileModel> _allProfiles;
        private static ProfileHandler _handler;
        private static object _lockObject = new object();
        private ApiLink<ProfileModel> apiLink;

        /// <summary>
        /// This contains the currently logged in profile.
        /// </summary>
        public ProfileModel CurrentLoggedInProfile
        {
            get { return _currentLoggedInProfile; }
            private set
            {
                if (value == null)
                {
                    _currentLoggedInProfile = value;
                    if (LogOutEvent != null)
                    {
                        LogOutEvent();
                    }
                    return;
                }
                if (value == SelectedProfile) return;

                _currentLoggedInProfile = value;
                if (LogInEvent != null)
                {
                    LogInEvent();
                }

            }
        }


        /// <summary>
        /// Contains the currently selected profile. IE, could be used to hold a profile to delete.
        /// But is it nessesary.
        /// </summary>
        public ProfileModel SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                if (value == CurrentLoggedInProfile)
                {
                    _selectedProfile = null;
                }
                else
                {
                    _selectedProfile = value;
                }
            }
        }

        /// <summary>
        /// Contains all profiles.
        /// </summary>
        public IEnumerable<ProfileModel> AllProfiles
        {
            get
            {
                return _allProfiles;
            }
            set { _allProfiles = value; }
        }

        #endregion

        #region Singleton section
        /// <summary>
        /// Private constructor, to implement the singleton pattern
        /// </summary>
        private ProfileHandler()
        {
            apiLink = new ApiLink<ProfileModel>();
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

        #region Create method
        /// <summary>
        /// This method adds a new user to the profile. By default, it will be an Activated User that has not been verified.
        /// </summary>
        /// <param name="profile">Pass in a ProfileModel with the state that you wish to create the profile in.</param>
        public async void Create(ProfileModel profile)
        {
            if (profile == null) return;
            profile.FK_ProfileLevel = (int)ProfileLevel.User;
            profile.FK_ProfileStatus = (int)ProfileStatus.Active;
            profile.ProfileIsVerified = false;
            profile.FK_ProfileVerificationType = (int)ProfileVerificationType.NotVerified;
            profile.ProfileCanReport = 1;

            profile.ProfilePasswordSalt = GenerateSalt();
            profile.ProfilePassword = HashPassword(profile.ProfilePassword, profile.ProfilePasswordSalt);

            using (var result = await apiLink.PostAsJsonAsync(profile))
            {
                if (result != null)
                {
                    await new MessageDialog(result.ReasonPhrase).ShowAsync();
                    if (CreationEvent != null)
                    {
                        CreationEvent(result.IsSuccessStatusCode);
                    }
                }
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
        public void Delete(ProfileModel profile)
        {
            DoStatusUpdate(
                profile,
                ProfileStatus.Delete,
                () => CurrentLoggedInProfile.FK_ProfileLevel < (int)ProfileLevel.Administrator,
                () => profile.ProfileId != CurrentLoggedInProfile.ProfileId
                );
        }

        #endregion

        #region Suspend method
        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to suspend a profile.
        /// And if so, change the status of the selected profile to suspended.
        /// If modifying the profile status fails, it will throw a <see cref="ProfileUpdate"/> exception.
        /// </summary>
        /// <param name="obj">The profile to suspend</param>
        public void Suspend(ProfileModel profile)
        {
            DoStatusUpdate(
                profile,
                ProfileStatus.Suspend,
                () => CurrentLoggedInProfile.FK_ProfileLevel < (int)ProfileLevel.Administrator,
                () => profile.ProfileId != CurrentLoggedInProfile.ProfileId
                );
        }

        #endregion

        #region Disable method
        /// <summary>
        /// This method will check if the <see cref="CurrentLoggedInProfile"/> has the rights to disable a profile.
        /// And if so, change the status of the selected profile to suspended.
        /// If modifying the profile status fails, it will throw a <see cref="ProfileUpdate"/> exception.
        /// </summary>
        /// <param name="obj">The profile to disable</param>
        public void Disable(ProfileModel profile)
        {
            DoStatusUpdate(
                profile,
                ProfileStatus.Disable,
                () => CurrentLoggedInProfile.FK_ProfileLevel < (int)ProfileLevel.User,
                () => profile.ProfileId == CurrentLoggedInProfile.ProfileId
            );
        }
        #endregion

        #region Update method
        /// <summary>
        /// This method will take the ProfileModel it is supplied, and update it on the server
        /// </summary>
        /// <param name="obj">A ProfileModel</param>
        public async void Update(ProfileModel profile)
        {
            if (profile == null) return;
            if (CurrentLoggedInProfile == null) return;
            using (var result = await apiLink.PutAsJsonAsync(profile, profile.ProfileId))
            {
                if (result != null)
                {
                    if (result.IsSuccessStatusCode)
                    {
                        await
                            new MessageDialog("The profile: " + profile.ProfileName + " was updated",
                                result.ReasonPhrase).ShowAsync();
                    }
                    else
                    {
                        await new MessageDialog("Update failed", result.ReasonPhrase).ShowAsync();
                    }

                    if (UpdateEvent != null)
                    {
                        UpdateEvent(result.IsSuccessStatusCode);
                    }
                }
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
        public async Task<IEnumerable<ProfileModel>> Search(ProfileModel needle)
        {
            IEnumerable<ProfileModel> haystack = null;
            using (var result = await apiLink.GetAsync())
            {
                if (result != null)
                {
                    if (result.IsSuccessStatusCode)
                    {
                        haystack = await result.Content.ReadAsAsync<IEnumerable<ProfileModel>>();
                    }
                    else
                    {
                        await new MessageDialog(result.ReasonPhrase).ShowAsync();
                    }
                }
            }

            if (needle.ProfileId != 0)
            {
                return haystack.Where(p => p.ProfileId == needle.ProfileId);
            }
            if (needle.ProfileName != null)
            {
                return haystack.Where(p => p.ProfileName.Contains(needle.ProfileName));
            }
            if (needle.ProfileEmail != null)
            {
                return haystack.Where(p => p.ProfileEmail == needle.ProfileEmail);
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
            if (profile.ProfilePassword == null || profile.ProfileName == null)
            {
                ErrorHandler.RequiredFields(new List<string> {"Username", "Password"});
                return;
            }
            using (var result = await apiLink.GetAsync())
            {
                // TODO update the webapi, so i dont have to request all the information like this.
                // All the user information is up for graps each time someone logs in.
                try
                {
                    if (result == null) return;
                    if (!result.IsSuccessStatusCode) return;
                    var listOfProfiles = await result.Content.ReadAsAsync<IEnumerable<ProfileModel>>();
                    try
                    {
                        var selectedProfile =
                            listOfProfiles.FirstOrDefault(
                                p =>
                                    p.ProfileName.ToLower() == profile.ProfileName.ToLower() &&
                                    p.ProfilePassword ==
                                    HashPassword(profile.ProfilePassword, p.ProfilePasswordSalt));

                        if (selectedProfile.FK_ProfileStatus == (int) ProfileStatus.Active)
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
                catch (Exception e)
                {
                    new MessageDialog(e.Message).ShowAsync();
                    //ErrorHandler.NoResponseFromApi();
                }
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
        #region DoStatusUpdate(args)
        /// <summary>
        /// Used internally to make status updates to a profile. Used by, Disable, Delete and Suspend
        /// </summary>
        /// <param name="profile">The profile to modify</param>
        /// <param name="newStatus">The status to give it</param>
        /// <param name="profileLevelCheck">Used in an if statement to check if the <see cref="CurrentLoggedInProfile"/> has the correct level. Should evaluate to false to pass</param>
        /// <param name="targetProfile">Used in an if statement to check if the action is allowed on the target profile. Should evaluate to true to pass</param>
        private async void DoStatusUpdate(ProfileModel profile, ProfileStatus newStatus, Func<bool> profileLevelCheck, Func<bool> targetProfile)
        {
            if (profile == null) return;
            if (CurrentLoggedInProfile == null) return;
            if (profileLevelCheck.Invoke())
            {
                ErrorHandler.WrongProfileLevel((ProfileLevel)CurrentLoggedInProfile.FK_ProfileLevel, newStatus.ToString());
            }
            else
            {
                if (targetProfile.Invoke())
                {
                    profile.FK_ProfileStatus = (int)newStatus;

                    using (var result = await apiLink.PutAsJsonAsync(profile,profile.ProfileId))
                    {
                        if (result != null)
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                await new MessageDialog(profile.ProfileName + " has been " + newStatus).ShowAsync();
                            }
                            else
                            {
                                await new MessageDialog("Update failed", result.ReasonPhrase).ShowAsync();
                            }
                            if (UpdateEvent != null)
                            {
                                UpdateEvent(result.IsSuccessStatusCode);
                            }
                        }
                    }
                }
                else
                {
                    ErrorHandler.WrongTargetProfile(newStatus.ToString());
                }
            }
        }
        #endregion

        #region GetAllProfiles()
        /// <summary>
        /// Using this method will update the content of <see cref="AllProfiles"/>
        /// </summary>
        public async void GetAllProfiles()
        {
            if (CurrentLoggedInProfile == null) return;
            using (var result = await apiLink.GetAsync())
            {
                if (result == null) return;
                if (!result.IsSuccessStatusCode) return;
                try
                {
                    AllProfiles = await result.Content.ReadAsAsync<IEnumerable<ProfileModel>>();
                }
                catch (Exception e)
                {
                    new MessageDialog(e.Message).ShowAsync();
                }
            }
        }
        #endregion

        #region GenerateSalt()
        /// <summary>
        /// This will generate a cryptographic grade random string
        /// </summary>
        /// <returns>Random string</returns>
        public long GenerateSalt()
        {
            //This is a very weak salt. The database was set up incorrectly.
            // TODO: Change the EF generated models in API, to work with a better database data type
            string randomnumber = CryptographicBuffer.GenerateRandomNumber().ToString();
            string eightDigits = "";
            for (int i = 0; i < 8; i++)
            {
                eightDigits += randomnumber[i];
            }
            return Convert.ToInt64(eightDigits); //CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom(64));
        }
        #endregion

        #region HashPassword()
        /// <summary>
        /// This method hashes the input, for use as a password.
        /// </summary>
        /// <param name="password">The string you wish to Hash</param>
        /// <param name="salt">The salt</param>
        /// <returns>The hashed result of the two inputs</returns>
        private string HashPassword(string password, long salt)
        {
            var passwordAndSalt = password + salt;
            var provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            var hash = provider.CreateHash();

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(passwordAndSalt, BinaryStringEncoding.Utf8);
            hash.Append(buffer);
            IBuffer hashedBuffer = hash.GetValueAndReset();

            return CryptographicBuffer.EncodeToBase64String(hashedBuffer);
        }
        #endregion

        #endregion
    }

}