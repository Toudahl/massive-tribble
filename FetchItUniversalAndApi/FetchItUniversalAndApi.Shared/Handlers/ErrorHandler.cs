using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;

namespace FetchItUniversalAndApi.Handlers
{
    class ErrorHandler
    {
        private static Object _lockObject = new object();
        private static ErrorHandler _handler;

        public static ErrorHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new ErrorHandler();
                }
                return _handler;
            }
        }

        #region ErrorMessageMethods
        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that a wrong model was passed to a method.
        /// </summary>
        /// <param name="objSupplied">The object supplied to the method.</param>
        /// <param name="objNeeded">The object the method is expecting.</param>
        public void WrongModelError(object objSupplied, object objNeeded)
        {
            string messageToUser = "MESSAE TO DEVELOPER: A wrong model was supplied. You tried to pass " + objSupplied.GetType().Name + " instead of " + objNeeded.GetType().Name;
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }

        /// <summary>
        /// Sends an async message to the user, telling him that a GET operation went wrong.
        /// </summary>
        /// <param name="obj">The object that needs to be retrieved from the database.</param>
        public void GettingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with getting the " + obj.GetType().Name + "s from the database, please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }

        /// <summary>
        /// Sends an async message to the user, telling him that a POST operation went wrong.
        /// </summary>
        /// <param name="obj">The object to POST.</param>
        public void CreatingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with posting the " + obj.GetType().Name + " to the database, please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }

        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that a PUT operation went wrong.
        /// </summary>
        /// <param name="obj">The object to PUT.</param>
        public void UpdatingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with updating the " + obj.GetType().Name + ", please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }

        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that a DELETE operation went wrong.
        /// </summary>
        /// <param name="obj">The object to DELETE.</param>
        public void DeletingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }

        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that suspending the object did not work.
        /// </summary>
        /// <param name="obj">The object to suspend.</param>
        public void SuspendingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }


        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that disabling the object did not work.
        /// </summary>
        /// <param name="obj">The object to disable.</param>
        public void DisablingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }

        /// <summary>
        /// FOR DEVELOPER: Sends an async message to the user, telling him that activating the object did not work.
        /// </summary>
        /// <param name="obj">The object to activate.</param>
        public void ActivatingError(object obj)
        {
            string messageToUser = "MESSAE TO DEVELOPER: Something went wrong with deleting the " + obj.GetType().Name + " from the database, please try again.";
            MessageDialog message = new MessageDialog(messageToUser);
            message.ShowAsync();
        }
        #endregion
    }
}
