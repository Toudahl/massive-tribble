using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Lárus Þór Jóhannsson
    /// <summary>
    /// Handles getting and creating messages sent to users.
    /// Messages includes Feedbacks, Comments (to Tasks) and Notifications
    /// E-mail handling IS NOT IMPLEMENTED and not currently planned.
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
        private static TaskHandler _th = TaskHandler.GetInstance();
        private static ProfileHandler _ph = ProfileHandler.GetInstance();
        #endregion

        #region Methods
        #region Constructor
        static MessageHandler()
        {
            msgWebClient.BaseAddress = new Uri(serverLocation);
            msgWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Pertaining to Feedbacks
        /// <summary>
        /// Creates a FeedbackModel object WITHOUT a comment and POSTs it to the database server by parsing it to json.
        /// </summary>
        /// <param name="rating">A number between 1 and 10.</param>
        /// <param name="fromTask">The Task that the Feedback is assigned to.</param>
        //TODO: Make return type Task
        public static async void CreateFeedback(int rating, TaskModel fromTask)
        {
            if (rating < 1 || rating > 10)
            {
                MessageDialog errorDialogWrongInpt = new MessageDialog("Rating is out of bounds. Please enter a number from 1 to 10.", "Rating out of bounds.");
                errorDialogWrongInpt.ShowAsync();
            }
            #region Build Feedback
            FeedbackModel createdFeedback = new FeedbackModel();
            createdFeedback.FK_FeedbackForTask = fromTask.TaskId;
            createdFeedback.FK_FeedbackStatus = (int)FeedbackStatus.Active;
            #endregion
            createdFeedback.FeedbackRating = (byte)rating;
            try
            {
                await msgWebClient.PostAsJsonAsync("FeedbackModels", createdFeedback);
            }
            catch (Exception)
            {
                ErrorHandler.CreatingError(createdFeedback);
            }
        }

        /// <summary>
        /// Creates a FeedbackModel object WITH a comment and POSTs it to the database server by parsing it to json.
        /// </summary>
        /// <param name="rating">A number between 1 and 10.</param>
        /// <param name="feedbackComment">The comment assigned to the rating (if null, use the other overload)</param>
        /// <param name="fromTask">The Task that the Feedback is assigned to.</param>
        //TODO: Make return type Task
        public static async void CreateFeedback(int rating, string feedbackComment, TaskModel fromTask)
        {
            if (rating < 1 || rating > 10)
            {
                MessageDialog errorDialogWrongInpt = new MessageDialog("Rating is out of bounds. Please enter a number from 1 to 10.", "Rating out of bounds.");
                errorDialogWrongInpt.ShowAsync();
            }
            #region Build Feedback
            FeedbackModel createdFeedback = new FeedbackModel();
            createdFeedback.FeedbackComment = feedbackComment;
            createdFeedback.FK_FeedbackForTask = fromTask.TaskId;
            createdFeedback.FK_FeedbackStatus = (int)FeedbackStatus.Active;
            #endregion
            createdFeedback.FeedbackRating = (byte)rating;
            try
            {
                await msgWebClient.PostAsJsonAsync("FeedbackModels", createdFeedback);
            }
            catch (Exception)
            {
                ErrorHandler.CreatingError(createdFeedback);
            }
        }

        /// <summary>
        /// A method that returns a collection of all Feedback objects with a certain status
        /// </summary>
        /// <returns> A IENumerable collection of Feedback objects</returns>
        /// <param name="status">Tells the API which Feedbacks you want to get (Active/Disabled...)</param>
        //TODO: Make async and return type Task
        public static IEnumerable<FeedbackModel> GetFeedback(FeedbackStatus status)
        {
            try
            {
                var reports = Task.Run(async () => await msgWebClient.GetAsync("FeedbackModels"));
                return reports.Result.Content.ReadAsAsync<IEnumerable<FeedbackModel>>().Result.Where(r => r.FK_FeedbackStatus == (int)status);
            }
            catch (Exception exception)
            {
                ErrorHandler.GettingError(new FeedbackModel());
                return null;
            }
        }

        /// <summary>
        /// A method that returns a collection of all Feedback objects with a certain status and to a certain task
        /// </summary>
        /// <returns> A IENumerable collection of Feedback objects</returns>
        /// <param name="status">Tells the API which Feedbacks you want to get (Active/Disabled...)</param>
        /// <param name="forTask">The task which you want to get Feedbacks from</param>
        //TODO: Make async and return type Task
        public static IEnumerable<FeedbackModel> GetFeedback(FeedbackStatus status, TaskModel forTask)
        {
            try
            {
                var reports = Task.Run(async () => await msgWebClient.GetAsync("FeedbackModels"));
                return reports.Result.Content.ReadAsAsync<IEnumerable<FeedbackModel>>().Result.Where(r => (r.FK_FeedbackStatus == (int)status || r.FK_FeedbackForTask == forTask.TaskId));
            }
            catch (Exception)
            {
                ErrorHandler.GettingError(new FeedbackModel());
                return null;
            }
        }

        /// <summary>
        /// A method that returns a collection of all Feedback objects with a certain status and from a certain profile
        /// </summary>
        /// <returns> A IENumerable collection of Feedback objects</returns>
        /// <param name="status">A filter for what feedback status you want returned</param>
        /// <param name="feedbackProfile">Tells the API which Feedbacks you want to get (Active/Disabled...)</param>
        //TODO: Make async and return type Task
        public static IEnumerable<FeedbackModel> GetFeedback(FeedbackStatus status, ProfileModel feedbackProfile)
        {
            try
            {
                var tasks = _th.GetTasks(TaskHandler.TaskStatus.Completed);
                var reportsStream = Task.Run(async () => await msgWebClient.GetAsync("FeedbackModels"));
                IEnumerable<FeedbackModel> feedbacks = reportsStream.Result.Content.ReadAsAsync<IEnumerable<FeedbackModel>>().Result;
                //Takes tasks and feedback collections, joins them and returns feedbacks from tasks where the profile provided is a fetcher
                var returnedFeedbacks = from task in tasks
                                        join feedback in feedbacks
                                            on task.TaskId equals feedback.FK_FeedbackForTask
                                        where task.FK_TaskFetcher == feedbackProfile.ProfileId
                                        select feedback;
                return returnedFeedbacks;
            }
            catch (Exception)
            {
                ErrorHandler.GettingError(new FeedbackModel());
                return null;
            }
        }
        #endregion

        #region Pertaining to TaskComments
        /// <summary>
        /// Creates a CommentModel item in the corresponding Task.
        /// Sets the time created and the author of it.
        /// </summary>
        /// <param name="toTask">The task to comment on</param>
        /// <param name="comment">The comment</param>
        /// <param name="authorProfile">The profile posting the comment</param>
        //TODO: Make return type Task
        public static async void CreateTaskComment(TaskModel toTask, string comment, ProfileModel authorProfile)
        {
            try
            {
                var updatedTaskStream = Task.Run(async () => await msgWebClient.GetAsync("TaskModels/" + toTask.TaskId));
                var updatedTask = updatedTaskStream.Result.Content.ReadAsAsync<TaskModel>().Result;
                #region Build Comment
                CommentModel newComment = new CommentModel();
                newComment.CommentText = comment;
                newComment.CommentTimeCreated = DateTime.UtcNow;
                newComment.FK_CommentTask = toTask.TaskId;
                newComment.FK_CommentCreator = authorProfile.ProfileId;
                #endregion
                updatedTask.Comments.Add(newComment);
                try
                {
                    await msgWebClient.PostAsJsonAsync("commentmodels", newComment);
                }
                catch (Exception)
                {
                    ErrorHandler.UpdatingError(updatedTask);
                }
            }
            catch (Exception)
            {
                ErrorHandler.CreatingError(new CommentModel());
            }
        }

        /// <summary>
        /// Returns IENumerable of Task Comments from the Task provided.
        /// If there are no Comments then it returns an empty IENumerable.
        /// </summary>
        /// <param name="fromTask">The Task that you want the comments from</param>
        /// <returns>IENumerable of CommentModels</returns>
        public static async Task<IEnumerable<CommentModel>> GetTaskComments(TaskModel fromTask)
        {
            try
            {
                var updatedTaskStream = Task.Run(async () => await msgWebClient.GetAsync("CommentModels"));
                return
                    updatedTaskStream.Result.Content.ReadAsAsync<IEnumerable<CommentModel>>()
                        .Result.Where(c => c.FK_CommentTask == fromTask.TaskId);
            }
            catch (Exception)
            {
                ErrorHandler.GettingError(new CommentModel());
                return null;
            }
        }
        #endregion

        #region Pertaining to Notifications
        /// <summary>
        /// Creates a Notification object and POSTs it to the database server by parsing it to json.
        /// </summary>
        /// <param name="notification">The Notification to Send</param>
        /// NotificationModel with everything inputted except NotificationId (should be null!) FK_NotificationStatus and NotificationSent.
        //TODO: Make return type Task
        public static async void SendNotification(NotificationModel notification)
        {
            #region Build Notification
            notification.FK_NotificationStatus = 1;
            notification.NotificationSent = DateTime.UtcNow;
            #endregion
            try
            {
                await msgWebClient.PostAsJsonAsync("NotificationModels", notification);
            }
            catch (Exception)
            {
                ErrorHandler.CreatingError(notification);
            }
        }

        /// <summary>
        /// A method that returns a collection of all Notification objects sent to or from the CurrentLoggedInProfile
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
                        .Where(n => n.FK_NotificationFrom == _ph.CurrentLoggedInProfile.ProfileId
                        || n.FK_NotificationTo == _ph.CurrentLoggedInProfile.ProfileId);
            }
            catch
            {
                ErrorHandler.GettingError(new NotificationModel());
                return null;
            }
        }
        #endregion

        #region Pertaining to E-mails (NOT IMPLEMENTED)
        /// <summary>
        /// NOT WORKING. Decided it's not high priority and complicated to implement.
        /// It should also be server-side anyways.
        /// Sends an E-mail from the system. Doesn't add any text to the message body.
        /// </summary>
        /// <param name="email">The e-mail to send</param>
        //TODO: Make return type Task
        public static async void SendEmail(EmailModel email, EmailType emailType, ProfileModel receivingProfile)
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
            throw new NotImplementedException("This has been scrapped. Deemed as low priority. Contact Lárus Þór if you encounter this.");
        }
        #endregion
        #endregion
    }
}
