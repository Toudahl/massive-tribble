using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    internal class TaskDetailViewModel
    {
        public TaskHandler TaskHandler { get; set; }
        public ProfileHandler ProfileHandler { get; set; }
       
        public ProfileModel Taskmaster { get; set; }
        public ProfileModel Fetcher { get; set; }
        public TaskModel SelectedTask { get; set; }
        public TaskHandler.TaskStatus TaskStatus {get; set;}
        
        public TaskDetailViewModel()
        {
            TaskHandler = TaskHandler.GetInstance();
            ProfileHandler = ProfileHandler.GetInstance();

            //Needs to check if selected task is null or not, or there is a "Reference not set to an Object"
            //error on the TaskDetailPage
            if (TaskHandler.SelectedTask != null)
            {
                SelectedTask = TaskHandler.SelectedTask;
                
                //Sets the taskmaster
                var profileOfTm =
                    ProfileHandler.Search(new ProfileModel() {ProfileId = SelectedTask.FK_TaskMaster}).ToArray().First()
                        as ProfileModel;
                Taskmaster = profileOfTm;
                
                //Sets the Fetcher (Needs this check, or else a Exception is thrown)
                var profileOfFetcher =
                ProfileHandler.Search(new ProfileModel() {ProfileId = (int) SelectedTask.FK_TaskFetcher})
                    .ToArray()
                    .First() as ProfileModel;
            
                if (profileOfFetcher != null)
                {
                    Fetcher = profileOfFetcher;
                }

                TaskStatus = (TaskHandler.TaskStatus)SelectedTask.FK_TaskStatus;
            }
      }
   }
}
