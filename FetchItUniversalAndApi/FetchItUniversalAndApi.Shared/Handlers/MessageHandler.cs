using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    static class MessageHandler
    {
        public static void CreateFeedback(int rating, string optionalText = "")
        {
            throw new NotImplementedException();
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
