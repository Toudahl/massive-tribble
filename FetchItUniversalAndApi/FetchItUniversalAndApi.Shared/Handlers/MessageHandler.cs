using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
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
            DeleteDeleted = 2,
            Suspended = 3,
            Disabled = 4,
            Active = 5,
            Unactivated = 6
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
                throw new Exception("Rating is out of bounds. Must be between 1 and 10. Contact Þór for free cookies and milk for discovering the bug.");
            }
            FeedbackModel createdFeedback = new FeedbackModel();
            createdFeedback.FeedbackComment = optionalText;
            createdFeedback.FK_FeedbackForTask = fromTask.TaskId;
            createdFeedback.FK_FeedbackStatus = (int) FeedbackStatus.Active;
            try
            {
                createdFeedback.FeedbackRating = (byte)rating;
            }
            catch (Exception)
            {
                throw new InvalidCastException("The rating parameter provided is not castable to byte.");
            }
            try
            {
                msgWebClient.PostAsJsonAsync("FeedbackModels", createdFeedback);
            }
            catch (Exception)
            {
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
                    throw exception;
                }
            
        }

        public static void CreateTaskComment(TaskModel toTask)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns IENumerable of Task Comments from the Task provided.
        /// If there are no Comments then it returns an empty IENumerable.
        /// </summary>
        /// <param name="fromTask"></param>
        /// The Task ti
        /// <returns></returns>
        public static IEnumerable<CommentModel> GetTaskComments(TaskModel fromTask)
        {
            try
            {
                var updatedTask = Task.Run(async () => await msgWebClient.GetAsync("TaskModels/" + fromTask.TaskId));
                var taskComments = updatedTask.Result.Content.ReadAsAsync<TaskModel>().Result;
                return taskComments.Comments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SendEmail(EmailModel email)
        {
            throw new NotImplementedException();
        }

        public static void SendNotification(NotificationModel notification)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<NotificationModel> GetNotifications()
        {
            throw new NotImplementedException();
        }
    #endregion
    }
}
