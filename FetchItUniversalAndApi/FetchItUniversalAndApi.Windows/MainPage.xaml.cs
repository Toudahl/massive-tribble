using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static HttpClient msgWebClient = new HttpClient();
        private static readonly string serverLocation = "http://fetchit.mortentoudahl.dk/api/";
        private int rating = 3;
        private string comment = "lol, this sucks";
        private IEnumerable<TaskModel> taskList;
        public MainPage()
        {
            this.InitializeComponent();
            msgWebClient.BaseAddress = new Uri(serverLocation);
            msgWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var reports = Task.Run(async () => await msgWebClient.GetAsync("TaskModels"));
                taskList = reports.Result.Content.ReadAsAsync<IEnumerable<TaskModel>>().Result;
            }
            catch (Exception)
            {

                throw;
            }
            TaskModel testTask = taskList.ElementAt(1);
            //MessageHandler.CreateFeedback(rating, comment, testTask);
            //IEnumerable<FeedbackModel> testFeedbacks = MessageHandler.GetFeedback(MessageHandler.FeedbackStatus.Active);
            //List<FeedbackModel> testFeedbackList = testFeedbacks.ToList();
            var testVar = MessageHandler.GetTaskComments(testTask);
        }
    }
}
