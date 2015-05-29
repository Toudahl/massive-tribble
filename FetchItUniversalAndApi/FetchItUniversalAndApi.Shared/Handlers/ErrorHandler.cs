using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    // Author : Morten Toudahl
    static class ErrorHandler
    {
        #region Fields
        private static string _messageToUser;
        private const string FailedToContactApi = "Failed to contact the web server";
        private const string FailedToLogIn = "Failed to log in";
        private const string RequiredInput = "More input required";
        private const string NotAllowed = "Not allowed";
        private const string OutOfBounds = "Out of bounds";
        private const string EditTask = "Edit Task";

        private static LogHandler _lh;

        #endregion

        #region LogEvent(message)
        /// <summary>
        /// This will log the event.
        /// </summary>
        /// <param name="message">The message that the user also sees.</param>
        private static async void LogEvent(string message)
        {
            _lh = LogHandler.GetInstance();
            var logMessage = message + "\nIP associated with the attempt: " + await GetExternalIp();
            _lh.Create(new LogModel { LogMessage = logMessage });
        }
        #endregion

        #region GetExternalIp()
        /// <summary>
        /// This will get the IP of the user, if possible
        /// </summary>
        /// <returns>IP</returns>
        private static async Task<string> GetExternalIp()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync("http://canihazip.com/s");
                }
                catch (WebException)
                {
                    // this one is offline
                }

                try
                {
                    return await client.GetStringAsync("http://wtfismyip.com/text");
                }
                catch (WebException)
                {
                    // offline...
                }

                try
                {
                    return await client.GetStringAsync("http://ip.telize.com/");
                }
                catch (WebException)
                {
                    // offline too...
                }

                // if we got here, all the websites are down, which is unlikely
                return "No ip found";
            }
        }
        #endregion

        #region DisplayErrorMessage(message, title)
        /// <summary>
        /// This will display a <see cref="MessageDialog"/> to the user.
        /// Use this, if none of the predefined methods apply to the error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="title">Title</param>
        public static async void DisplayErrorMessage(string message, string title)
        {
            await new MessageDialog(message, title).ShowAsync();
        }
        #endregion

        #region WrongModelError(objSupplied, objNeeded)
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that a wrong model was passed to a method.
        /// </summary>
        /// <param name="objSupplied">The object supplied to the method.</param>
        /// <param name="objNeeded">The object the method is expecting.</param>
        public static void WrongModelError(object objSupplied, object objNeeded)
        {
            _messageToUser = "MESSAE TO DEVELOPER: A wrong model was supplied. You tried to pass " + objSupplied.GetType().Name + " instead of " + objNeeded.GetType().Name;
            DisplayErrorMessage(_messageToUser, "Wrong model error");
        }
        #endregion

        #region GettingError(obj)
        /// <summary>
        /// Sends an async message to the user, telling him that a GET operation went wrong.
        /// </summary>
        /// <param name="obj">The object that needs to be retrieved from the database.</param>
        public static void GettingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with getting the " + obj.GetType().Name + "s from the database, please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region CreatingError(obj)
        /// <summary>
        /// Sends an async message to the user, telling him that a POST operation went wrong.
        /// </summary>
        /// <param name="obj">The object to POST.</param>
        public static void CreatingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with posting the " + obj.GetType().Name + " to the database, please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region UpdatingError(obj)
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that a PUT operation went wrong.
        /// </summary>
        /// <param name="obj">The object to PUT.</param>
        public static void UpdatingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with updating the " + obj.GetType().Name + ", please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region DeletingError(obj)
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that a DELETE operation went wrong.
        /// </summary>
        /// <param name="obj">The object to DELETE.</param>
        public static void DeletingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region SuspendingError(obj)
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that suspending the object did not work.
        /// </summary>
        /// <param name="obj">The object to suspend.</param>
        public static void SuspendingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region DisablingError(obj)
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that disabling the object did not work.
        /// </summary>
        /// <param name="obj">The object to disable.</param>
        public static void DisablingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region ActivatingError(obj)
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that activating the object did not work.
        /// </summary>
        /// <param name="obj">The object to activate.</param>
        public static void ActivatingError(object obj)
        {
            _messageToUser = "MESSAE TO DEVELOPER: Something went wrong with activating the " + obj.GetType().Name + " from the database, please try again.";
            DisplayErrorMessage(_messageToUser, FailedToContactApi);
        }
        #endregion

        #region WrongProfileStatus()
        /// <summary>
        /// Calling this method will display a message dialog to the user, informing that the profile is not active and unable to login.
        /// </summary>
        public static void WrongProfileStatus()
        {
            _messageToUser = "Your profile is not active, so you cannot log in";
            DisplayErrorMessage(_messageToUser, FailedToLogIn);
        }
        #endregion

        #region FailedLogIn(profileName)
        /// <summary>
        /// Calling this method will display a message dialog to the user, stating that the login failed due to password/username issues.
        /// </summary>
        /// <param name="profileName">Name of profile</param>
        public static void FailedLogIn(string profileName)
        {
            _messageToUser = "Profile (" + profileName + ") and password combination not found.\nLogin attempt has been logged.";
            DisplayErrorMessage(_messageToUser, FailedToLogIn);
            LogEvent(_messageToUser);
        }
        #endregion

        #region NoResponseFromApi()
        /// <summary>
        /// Calling this method will display a message dialog to the user, stating that the API did not respond.
        /// </summary>
        public static void NoResponseFromApi()
        {
            DisplayErrorMessage("Failed to contact the web api", FailedToContactApi);
        }
        #endregion

        #region RequiredFields(fields)
        /// <summary>
        /// Calling this method will display a message dialog to the user, stating that not all of the required fields was filled out.
        /// </summary>
        /// <param name="fields">List that contains the fields names.</param>
        public static void RequiredFields(List<string> fields)
        {
            _messageToUser = "You must fill out all required fields:";
            if (fields == null) return;
            foreach (var field in fields)
            {
                _messageToUser += "\n" + field;
            }
            DisplayErrorMessage(_messageToUser, RequiredInput);
        }
        #endregion

        #region WrongTargetProfile(action)
        /// <summary>
        /// Calling this method will display a message dialog to the user, informing that the desired action cannot be performed on the target profile
        /// </summary>
        /// <param name="action">The action that was attempted</param>
        public static void WrongTargetProfile(string action)
        {
            DisplayErrorMessage("You are not allowed to " + action + " this profile", NotAllowed);
        }
        #endregion

        #region WrongProfileLevel(profilelevel, action)
        /// <summary>
        /// Calling this method will display a message dialog to the user, informing that the profile level of the user was not high enough for the desired action.
        /// </summary>
        /// <param name="profileLevel">Actual profile level</param>
        /// <param name="action">The desired action</param>
        public static void WrongProfileLevel(ProfileHandler.ProfileLevel profileLevel, string action)
        {
            DisplayErrorMessage("Your profile level: " + profileLevel + " is not high enough to delete a "+action+".", NotAllowed);
        }
        #endregion

        #region RatingOutOfBounds()
        /// <summary>
        /// Calling this method will display a message dialog to the user, informing that the selected rating was out of bounds.
        /// </summary>
        public static void RatingOutOfBounds()
        {
            DisplayErrorMessage("Rating is out of bounds. Please enter a number from 1 to 10.", OutOfBounds);
        }
        #endregion

        #region CannotEditTask()
        /// <summary>
        /// Calling this method will display a message dialog to the user, informing that the selected task cannot be edited.
        /// </summary>
        public static void CannotEditTask()
        {
            DisplayErrorMessage("You cannot edit the task in its current condition.", EditTask);
        }
        #endregion
    }
}
