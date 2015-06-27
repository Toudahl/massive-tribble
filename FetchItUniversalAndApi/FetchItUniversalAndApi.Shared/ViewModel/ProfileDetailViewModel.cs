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
		public ProfileHandler ph { get; set; }

	    public ProfileModel DisplayedProfile
	    {
	        get
	        {
	            if (ph.SelectedProfile == null)
	            {
	                return ph.CurrentLoggedInProfile;
	            }
	            return ph.SelectedProfile;
	        }
	    }

	    public string selectedprofileName { get; set; }
		public string selectedprofileMobile { get; set; }
		public string selectedprofileAddress { get; set; }
		public string selectedprofileEmail { get; set; }
		public int selectedprofileId { get; set; }
		public string ProfileRating { get; set; }
		public string selectedProfileDescription { get; set; }
		public string IsProfileVerified { get; set; }
		public TaskHandler TaskHandler { get; set; }
		public IEnumerable<TaskModel> AllTasks { get; set; }
		#endregion



		public ProfileDetailViewModel()
		{
			ph = ProfileHandler.GetInstance();
			TaskHandler = TaskHandler.GetInstance();

            IsProfileVerified = DisplayedProfile.ProfileIsVerified == false ? "No" : "Yes";
            ProfileRating = "Rating: " + GetAverageRating();
		}

        /// <summary>
        /// Gets all the tasks of current user
        /// </summary>
        /// <returns></returns>
		public IEnumerable<TaskModel> GetCurrentUsersTasks()
		{
			return
				TaskHandler.GetTasks(TaskHandler.TaskStatus.Active)
				.Where(task => task.FK_TaskFetcher == ph.CurrentLoggedInProfile.ProfileId);
		}

        /// <summary>
        /// Gets the average rating of a user based on all rating he has received.
        /// </summary>
        /// <returns></returns>
		public double GetAverageRating()
		{
			var allfeedbacks = MessageHandler.GetFeedback(MessageHandler.FeedbackStatus.Active);

			var usersFeedbacks = from task in GetCurrentUsersTasks()
								 join feedback in allfeedbacks
									 on task.TaskId equals feedback.FK_FeedbackForTask
								 select new
								 {
									 Rating = feedback.FeedbackRating
								 };

			try
			{
				return usersFeedbacks.Average(f => f.Rating);
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