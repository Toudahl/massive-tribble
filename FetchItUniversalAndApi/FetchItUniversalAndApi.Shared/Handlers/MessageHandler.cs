using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.Data.Json;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{

    /// <summary>
    /// Handles getting and creating messages sent to users.
    /// </summary>
    static class MessageHandler
    {
        //The httpclient should probably be one object that all handlers call upon. There "shouldn't" be any reason to dispose of it or flush it.
        //But we should keep an eye out for if it's not closing the connections or not dumping the resources.
        private static HttpClient msgWebClient = new HttpClient();
        //TODO: This could probably be somewhere better and globally available. Also needs to be changed to refer to the intended server.
        private static string serverLocation = "http://localhost:1337";

        /// <summary>
        /// Creates a FeedbackModel object and sends it to the database server using POST by parsing it to json.
        /// </summary>
        /// <param name="rating"></param>
        /// The rating is not optional and must be a number between 1 and 10. It represents how the Taskmaster values the service that the Fetcher provided. 
        /// <param name="optionalText"></param>
        /// OptionalText is optional. Describes in more detail the service the Taskmaster provided.
        public static async void CreateFeedback(int rating, string optionalText)
        {
            if (rating < 1 || rating > 10)
            {
                throw new Exception("Rating is out of bounds. Must be between 1 and 10. Contact Þór for free cookies and milk for discovering the bug.");
            }
            msgWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            FeedbackModel createdFeedback = new FeedbackModel();
            createdFeedback.FeedbackComment = optionalText;
            try
            {
                createdFeedback.FeedbackRating = (byte)rating;
            }
            catch (Exception)
            {
                throw new InvalidCastException("The rating parameter provided is not castable to byte.");
            }
            //TODO: Add the Httpclient.PostAsJsonAsync to update the data on the database through the webApi. Missing the reference needed for webapi json calls 
            //TODO: The object being PUT needs to be formatted so that the webApi can put it in the right database.
            try
            {
                msgWebClient.PostAsJsonAsync(serverLocation, createdFeedback);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IEnumerable<FeedbackModel> GetFeedback()
        {
            throw new NotImplementedException();
        }

        public static void CreateTaskComment(TaskModel taskObj)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<CommentModel> GetTAskComments(TaskModel fromTask)
        {
            throw new NotImplementedException();
        }

        public static void SendEmail(EmailModel email)
        {
            throw new NotImplementedException();
        }

        public static void SendNotification(NotificationModel notification)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<NotificationModel> GetNoticications()
        {
            throw new NotImplementedException();
        }
    }
}
