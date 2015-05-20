using System;
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
		public bool FeedbackCreated { get; set; }

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

		#region Constructor
		public CreateFeedbackViewModel()
		{
			SuccessMessage = "Collapsed";
			FeedbackCreated = false;
			TaskHandler = TaskHandler.GetInstance();
			ProfileHandler = ProfileHandler.GetInstance();
			LoggedInProfile = ProfileHandler.CurrentLoggedInProfile;
			SelectedTask = TaskHandler.SelectedTask;

			SubmitFeedbackCommand = new RelayCommand(SubmitFeedback);

		}
		#endregion

		#region Methods
		private void SubmitFeedback()
		{
			if (FeedbackCreated)
			{
				MessageDialog message = new MessageDialog("You have already submitted a feedback for this task.", "Submit Feedback");
				message.ShowAsync();
			}
			else
			{
				MessageDialog message = new MessageDialog("Are you sure you want to submit this feedback?", "Submit Feedback");
				message.Commands.Add(new UICommand(
					"Yes",
					command => CreateTheFeedback()));

				message.Commands.Add(new UICommand(
					"No"));

				message.DefaultCommandIndex = 0;
				message.CancelCommandIndex = 1;

				message.ShowAsync();
			}
		}

		async public void CreateTheFeedback()
		{
			try
			{
				MessageHandler.CreateFeedback(Rating, OptionalText, SelectedTask);
				await Task.Delay(1000);
				OptionalText = "";
				Rating = 0;
				SuccessMessage = "Visible";
				FeedbackCreated = true;
				await Task.Delay(5000);
				SuccessMessage = "Collapsed";
			}
			catch (Exception)
			{
				throw;
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
