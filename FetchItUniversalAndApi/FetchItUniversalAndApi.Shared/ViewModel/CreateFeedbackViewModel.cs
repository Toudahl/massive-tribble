﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
	// Author: Kristinn Þór Jónsson
	class CreateFeedbackViewModel : INotifyPropertyChanged
	{
		#region Fields
		private string _optionalText;
		private int _rating;
		private string _successMessage;

		public TaskHandler TaskHandler { get; set; }
		public TaskModel SelectedTask { get; set; }
		public ProfileHandler ProfileHandler { get; set; }
		public ProfileModel LoggedInProfile { get; set; }
		public ICommand SubmitFeedbackCommand { get; set; }

		#endregion

		#region Constructor
		public CreateFeedbackViewModel()
		{
			//This success message string is binded two-way in the View, it makes a textbox
			//pop up, telling the user that Creating the feedback was successful, also used 
			//to navigate back.
			SuccessMessage = "Collapsed";

			TaskHandler = TaskHandler.GetInstance();
			ProfileHandler = ProfileHandler.GetInstance();
			LoggedInProfile = ProfileHandler.CurrentLoggedInProfile;
			SelectedTask = TaskHandler.SelectedTask;

			SubmitFeedbackCommand = new RelayCommand(SubmitFeedback);

		}
		#endregion

		#region Methods
		/// <summary>
		/// A method that shows the user a MessageDialog, giving him the option to submit a feedback he has created.
		/// </summary>
		async private void SubmitFeedback()
		{
			//A Check for the rating, which should be between 1 and 10, if not, the method does not continue.
			if (Rating < 1 || Rating > 10)
			{
				MessageDialog errorDialogWrongInpt = new MessageDialog("Rating is out of bounds. Please enter a number from 1 to 10.", "Rating out of bounds.");
				errorDialogWrongInpt.ShowAsync();
				return;
			}
			MessageDialog message = new MessageDialog("Are you sure you want to submit this feedback?", "Submit Feedback");
			message.Commands.Add(new UICommand(
				"Yes",
				command => CreateTheFeedback()));

			message.Commands.Add(new UICommand(
				"No"));

			message.DefaultCommandIndex = 0;
			message.CancelCommandIndex = 1;

			await message.ShowAsync();
		}

		/// <summary>
		/// This method calls the message handler to create and POST a feedback from the user to the database.
		/// </summary>
		async public void CreateTheFeedback()
		{
			try
			{
				await Task.Delay(500);
				MessageHandler.CreateFeedback(Rating, OptionalText, SelectedTask);
				SuccessMessage = "Visible";
				OptionalText = "";
				Rating = 0;
				await Task.Delay(5000);
				SuccessMessage = "Collapsed";
			}
			catch (Exception)
			{
				ErrorHandler.CreatingError(new FeedbackModel());
			}
		}
		#endregion

		#region Properties
		public string OptionalText
		{
			get { return _optionalText; }
			set
			{
				_optionalText = value;
				OnPropertyChanged();
			}
		}
		public int Rating
		{
			get { return _rating; }
			set
			{
				_rating = value;
				OnPropertyChanged();
			}
		}

		public string SuccessMessage
		{
			get { return _successMessage; }
			set
			{
				_successMessage = value;
				OnPropertyChanged();
			}
		}
		#endregion

		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
