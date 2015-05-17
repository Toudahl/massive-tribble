using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Lárus Þór Jóhannsson
    /// <summary>
    /// Handles getting and creating messages sent to users.
    /// </summary>
    static class MessageHandler
    {
        #region Enums
        public enum FeedbackStatus
        {
            Deleted = 2,
            Suspended = 3,
            Disabled = 4,
            Active = 5,
            Unactivated = 6
        }

        public enum NotificationStatus
        {
            Unread = 1,
            Read = 2,
            Disabled = 3,
        }

        /// <summary>
        /// The types of E-mails that the Handler can send.
        /// </summary>
        public enum EmailType
        {
            Activation,
            NewEmailVerification,
            NewPasswordVerification,
            NotificationEmail,
            AdministratorMessage,
        }
    #endregion

        #region Fields and Properties
        //The httpclient should probably be one object that all handlers call upon. There "shouldn't" be any reason to dispose of it or flush it.
        //But we should keep an eye out for if it's not closing the connections or not dumping the resources.
        private static HttpClient msgWebClient = new HttpClient();
        //TODO: This could probably be somewhere better and globally available. Also needs to be changed to refer to the intended server.
        private static readonly string serverLocation = "http://fetchit.mortentoudahl.dk/api/";
    #endregion

        #region Methods
        #region Constructor
        static MessageHandler()
        {
            msgWebClient.BaseAddress = new Uri(serverLocation);
            msgWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        /// <summary>
        /// Creates a FeedbackModel object and sends it to the database server using POST by parsing it to json.
        /// </summary>
        /// <param name="rating"></param>
        /// The rating is not optional and must be a number between 1 and 10. It represents how the Taskmaster values the service that the Fetcher provided. 
        /// <param name="optionalText"></param>
        /// OptionalText is optional. Describes in more detail the service the Taskmaster provided.
        /// <param name="fromTask"></param>
        /// The Task that the Feedback is assigned to. Get Profile info through here.
        public static async void CreateFeedback(int rating, string optionalText, TaskModel fromTask)
        {
            if (rating < 1 || rating > 10)
            {
                MessageDialog errorDialogWrongInpt = new MessageDialog("Rating is out of bounds. Must be between 1 and 10.", "Rating out of bounds.");
                errorDialogWrongInpt.ShowAsync();
            }
            FeedbackModel createdFeedback = new FeedbackModel();
            #region Build Feedback
            createdFeedback.FeedbackComment = optionalText;
            createdFeedback.FK_FeedbackForTask = fromTask.TaskId;
            createdFeedback.FK_FeedbackStatus = (int) FeedbackStatus.Active;
            #endregion
            try
            {
                createdFeedback.FeedbackRating = (byte)rating;
            }
            catch (Exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw new InvalidCastException("The rating parameter provided is not castable to byte.");
            }
            try
            {
                msgWebClient.PostAsJsonAsync("FeedbackModels", createdFeedback);
            }
            catch (Exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw;
            }
        }

        /// <summary>
        /// A method that returns a collection of all Feedback objects
        /// </summary>
        /// <returns> A IENumerable collection of Feedback objects</returns>
        /// <param name="status"></param>
        /// Tells the API which Feedbacks you want to get (Active/Disabled...)
        public static IEnumerable<FeedbackModel> GetFeedback(FeedbackStatus status)
        {
                try
                {
                    var reports = Task.Run(async () => await msgWebClient.GetAsync("FeedbackModels"));
                    return reports.Result.Content.ReadAsAsync<IEnumerable<FeedbackModel>>().Result;
                }
                catch (Exception exception)
                {
                    //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                    throw exception;
                }
            
        }

        /// <summary>
        /// Creates a CommentModel item in the corresponding Task.
        /// Sets the time created and the author of it.
        /// </summary>
        /// <param name="toTask"></param>
        /// <param name="comment"></param>
        /// <param name="authorProfile"></param>
        public static void CreateTaskComment(TaskModel toTask, string comment, ProfileModel authorProfile)
        {
            try
            {
                CommentModel newComment = new CommentModel();
                var updatedTaskStream = Task.Run(async () => await msgWebClient.GetAsync("TaskModels/" + toTask.TaskId));
                var updatedTask = updatedTaskStream.Result.Content.ReadAsAsync<TaskModel>().Result;
                #region Build Comment
                newComment.CommentText = comment;
                newComment.CommentTimeCreated = DateTime.UtcNow;
                newComment.FK_CommentTask = toTask.TaskId;
                newComment.FK_CommentCreator = authorProfile.ProfileId;
                newComment.Task = toTask;
                newComment.Profile = authorProfile;
                #endregion
                updatedTask.Comments.Add(newComment);
                try
                {
                    //TaskHandler.UpdateTask(updatedTask);
                }
                catch (Exception)
                {
                    //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                    throw;
                }
            }
            catch (Exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw;
            }
        }

        /// <summary>
        /// Returns IENumerable of Task Comments from the Task provided.
        /// If there are no Comments then it returns an empty IENumerable.
        /// </summary>
        /// <param name="fromTask"></param>
        /// The Task that you want the comments from
        /// <returns>IENumerable of CommentModel</returns>
        public static async Task<IEnumerable<CommentModel>> GetTaskComments(TaskModel fromTask)
        {
            try
            {
                var updatedTaskStream = Task.Run(async () => await msgWebClient.GetAsync("NotificationModels"));
                return updatedTaskStream.Result.Content.ReadAsAsync<TaskModel>().Result.Comments.Where(t => t.FK_CommentTask == fromTask.TaskId).ToObservableCollection();
            }
            catch (Exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw;
            }
        }

        /// <summary>
        /// NOT WORKING. Decided it's not high priority and complicated to implement.
        /// It should also be on the server anywyas.
        /// Sends an E-mail from the system. Doesn't add any text to the message body.
        /// </summary>
        /// <param name="email"></param>
        /// The EmailModel with receiving e-mail toAddress, subject and message
        public static void SendEmail(EmailModel email, EmailType emailType, ProfileModel receivingProfile)
        {
            #region Not working code, just for reference
            //string url = "http://fetchit.mortentoudahl.dk";
            //if (emailType == EmailType.Activation)
            //{
            //    url += "?" + emailType + "=" + receivingProfile.ProfileName;
            //    url += "&id=" + receivingProfile.ProfileAuthCode;
            //}
            //MailBuilder emailBuilder = new MailBuilder();
            //#region Build Email
            //emailBuilder.From.Add(new MailBox("zibat.fetchit@gmail.com","Fetch-It"));
            //emailBuilder.To.Add(new MailBox(receivingProfile.ProfileEmail, receivingProfile.ProfileName));
            //emailBuilder.Subject = email.Subject;
            //emailBuilder.Text = email.Message + url;
            //IMail emailSending = emailBuilder.Create();
            //#endregion
            //#region Send Email
            //using (Smtp smtp = new Smtp())
            //{
            //    smtp.Connect("smtp.google.com");

            //    ISendMessageResult result = smtp.SendMessage(emailSending);
            //    if (result.Status != SendMessageStatus.Success)
            //    {
            //        //TODO: Make it a custom exc.
            //        //throw new Exception("Omg e-mail sending didn't work!");
            //    }
            //    smtp.Close();
            //}

            //#endregion
            #endregion
            throw new NotImplementedException("Talk to Lárus Þór if you need this implemented. If you don't NEED this, think of this is low priority.");
        }

        /// <summary>
        /// Creates a Notification object and sends it to the database server using POST by parsing it to json.
        /// </summary>
        /// <param name="notification"></param>
        /// NotificationModel with everything inputted except NotificationId (should be null!) FK_NotificationStatus and NotificationSent.
        public static void SendNotification(NotificationModel notification)
        {
            #region Build Notification
            notification.FK_NotificationStatus = 1;
            notification.NotificationSent = DateTime.UtcNow;
            #endregion
            try
            {
                msgWebClient.PostAsJsonAsync("NotificationModels", notification);
            }
            catch (Exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw;
            }
        }

        /// <summary>
        /// A method that returns a collection of all Notification objects assigned to the CurrentLoggedInProfile
        /// </summary>
        /// <returns>IENumerable of NotificationModels</returns>
        public static async Task<IEnumerable<NotificationModel>> GetNotifications()
        {
            try
            {
                var notificationsStream = Task.Run(async () => await msgWebClient.GetAsync("NotificationModels"));
                var notificationStreamContent = notificationsStream.Result.Content;
                return
                    notificationStreamContent.ReadAsAsync<IEnumerable<NotificationModel>>()
                        .Result.Select(n => n)
                        .Where(n => n.FK_NotificationFrom == ProfileHandler.GetInstance().CurrentLoggedInProfile.ProfileId
                        || n.FK_NotificationTo == ProfileHandler.GetInstance().CurrentLoggedInProfile.ProfileId);
            }
            catch
            {
                MessageDialog gettingNotificationsError = new MessageDialog("Couldn't get Notifications");
                gettingNotificationsError.ShowAsync();
                return null;
            }
        }
    #endregion
    }
}
