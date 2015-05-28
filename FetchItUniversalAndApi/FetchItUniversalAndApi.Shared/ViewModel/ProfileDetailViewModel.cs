using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;


namespace FetchItUniversalAndApi.ViewModel
{
	class ProfileDetailViewModel : INotifyPropertyChanged
	{
		//Author: Jakub Czapski
		#region properties
		public ProfileHandler ProfileHandler { get; set; }
		public string selectedprofileName { get; set; }
		public string selectedprofileMobile { get; set; }
		public string selectedprofileAddress { get; set; }
		public string selectedprofileEmail { get; set; }
		public int selectedprofileId { get; set; }
		public string selectedProfileRating { get; set; }
		public string selectedProfileDescription { get; set; }
		public string IsProfileVerified { get; set; }
		public TaskHandler TaskHandler { get; set; }
		public IEnumerable<TaskModel> AllTasks { get; set; }
		#endregion



		public ProfileDetailViewModel()
		{
			ProfileHandler = ProfileHandler.GetInstance();
			TaskHandler = TaskHandler.GetInstance();

			selectedprofileName = ProfileHandler.SelectedProfile.ProfileName;
			selectedprofileMobile = ProfileHandler.SelectedProfile.ProfileMobile;
			selectedprofileEmail = ProfileHandler.SelectedProfile.ProfileEmail;
			selectedprofileAddress = ProfileHandler.SelectedProfile.ProfileAddress;
			selectedprofileId = ProfileHandler.SelectedProfile.ProfileId;
			selectedProfileDescription = ProfileHandler.SelectedProfile.ProfileText;
			IsProfileVerified = ProfileHandler.SelectedProfile.ProfileIsVerified == false ? "No" : "Yup";
			selectedProfileRating = "Rating: " + GetAverageRating();
		}

        /// <summary>
        /// Gets all the tasks of current user
        /// </summary>
        /// <returns></returns>
		public IEnumerable<TaskModel> GetCurrentUsersTasks()
		{
			return
				TaskHandler.GetTasks(TaskHandler.TaskStatus.Active)
				.Where(task => task.FK_TaskFetcher == ProfileHandler.CurrentLoggedInProfile.ProfileId);
		}
        /// <summary>
        /// Gets the average rating of a user based on all rating he has received.
        /// </summary>
        /// <returns></returns>
		public double GetAverageRating()
		{
			var allfedbacks = MessageHandler.GetFeedback(MessageHandler.FeedbackStatus.Active);

			var UsersFeedbacks = from task in GetCurrentUsersTasks()
								 join feedback in allfedbacks
									 on task.TaskId equals feedback.FK_FeedbackForTask
								 select new
								 {
									 Rating = feedback.FeedbackRating
								 };

			try
			{
				return UsersFeedbacks.Average(f => f.Rating);
			}
			catch (Exception)
			{

				return 0;
			}
		}

		#region prop changed
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}