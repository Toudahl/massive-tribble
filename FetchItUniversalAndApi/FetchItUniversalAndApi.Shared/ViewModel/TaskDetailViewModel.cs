using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
	//Author: Kristinn Þór Jónsson
	internal class TaskDetailViewModel : INotifyPropertyChanged
	{
		#region Fields
		private ObservableCollection<CommentModel> _commentsForTask;
		private TaskHandler.TaskStatus _taskStatus;

		private string _commentToLeave;
		private string _successMessage;

		public TaskHandler TaskHandler { get; set; }
		public ProfileHandler ProfileHandler { get; set; }

		public ProfileModel Taskmaster { get; set; }
		public ProfileModel Fetcher { get; set; }
		public TaskModel SelectedTask { get; set; }

		public string ItemPrice { get; set; }
		public string TaskFee { get; set; }

		public ICommand SaveChangesCommand { get; set; }
		public ICommand AssignToTaskCommand { get; set; }
		public ICommand ResignFromTaskCommand { get; set; }
		public ICommand MarkAsCompletedCommand { get; set; }
		public ICommand AddCommentCommand { get; set; }
		public bool Suspended { get; set; }
		#endregion

		#region Constructor
		public TaskDetailViewModel()
		{
			SuccessMessage = "Collapsed";

			TaskHandler = TaskHandler.GetInstance();
			ProfileHandler = ProfileHandler.GetInstance();

			SaveChangesCommand = new RelayCommand(SaveChanges);
			AssignToTaskCommand = new RelayCommand(AssignToTask);
			ResignFromTaskCommand = new RelayCommand(ResignFromTask);
			MarkAsCompletedCommand = new RelayCommand(MarkAsCompleted);
			AddCommentCommand = new RelayCommand(AddComment);

			//Needs to check if selected task is null or not, or there is a "Reference not set to an Object"
			//error on the TaskDetailPage
			if (TaskHandler.SelectedTask != null)
			{
				SelectedTask = TaskHandler.SelectedTask;

				//Sets the taskmaster
				var profileOfTm =
					ProfileHandler.Search(new ProfileModel() { ProfileId = SelectedTask.FK_TaskMaster }).ToArray().First()
						as ProfileModel;
				Taskmaster = profileOfTm;

				//Sets the Fetcher (Needs this check, or else a Exception is thrown)
				if (SelectedTask.FK_TaskFetcher != null)
				{
					var profileOfFetcher =
						ProfileHandler.Search(new ProfileModel() { ProfileId = (int)SelectedTask.FK_TaskFetcher })
							.ToArray()
							.First() as ProfileModel;

					if (profileOfFetcher != null)
					{
						Fetcher = profileOfFetcher;
					}
				}

				//Sets the feedbacks (if any)
				FeedbackModel feedbackForThisTask = null;
				var feedbacks = MessageHandler.GetFeedback(MessageHandler.FeedbackStatus.Active).ToList();
				try
				{
					feedbackForThisTask = feedbacks.Where(feedback => feedback.FK_FeedbackForTask == SelectedTask.TaskId)
						.Select(feedback => feedback).ToList().First();
				}
				catch (Exception)
				{
					;
				}

				//TODO: Is it needed? It is used in TaskDetailPage OnNavigatedTo
				if (feedbackForThisTask != null)
				{
					if (SelectedTask.Feedbacks.Count == 0)
					{
						SelectedTask.Feedbacks.Add(feedbackForThisTask);
					}
				}

				TaskStatus = (TaskHandler.TaskStatus)SelectedTask.FK_TaskStatus;

				//Adds all the comments through the MessageHandler
				CommentsForTask = MessageHandler.GetTaskComments(SelectedTask).Result.ToObservableCollection();

				ItemPrice = SelectedTask.TaskItemPrice.ToString();
				TaskFee = SelectedTask.TaskFee.ToString();
			}
		}

		#endregion

		#region Methods
		#region MessageMethods
		async public void SaveChanges()
		{
			MessageDialog message = new MessageDialog("Are you sure you want to save the changes?", "Update task");
			message.Commands.Add(new UICommand(
				"Yes",
				command => SaveChangesForTask()));

			message.Commands.Add(new UICommand(
				"No"));

			message.DefaultCommandIndex = 0;
			message.CancelCommandIndex = 1;

			await message.ShowAsync();
		}

		async public void AssignToTask()
		{
			if (SelectedTask.FK_TaskFetcher == ProfileHandler.CurrentLoggedInProfile.ProfileId)
			{
				MessageDialog alreadyResignedMessage = new MessageDialog("You have already assigned yourself to this task.", "Assign to task");
				await alreadyResignedMessage.ShowAsync();
			}
			else
			{
				MessageDialog message = new MessageDialog("Are you sure you want to assign yourself to this task?", "Assign to task");
				message.Commands.Add(new UICommand(
					"Yes",
					command => AssignProfileToTask()));

				message.Commands.Add(new UICommand(
					"No"));

				message.DefaultCommandIndex = 0;
				message.CancelCommandIndex = 1;

				await message.ShowAsync();
			}
		}

		async public void ResignFromTask()
		{
			if (SelectedTask.FK_TaskFetcher != ProfileHandler.CurrentLoggedInProfile.ProfileId)
			{
				MessageDialog alreadyResignedMessage = new MessageDialog("You have already resigned from this task.", "Resign from task");
				await alreadyResignedMessage.ShowAsync();
			}
			else if (SelectedTask.FK_TaskStatus != (int)TaskHandler.TaskStatus.Active)
			{
				MessageDialog alreadyResignedMessage = new MessageDialog("You can only resign yourself from a task if it is Active.", "Resign from task");
				await alreadyResignedMessage.ShowAsync();
			}
			else
			{
				MessageDialog message = new MessageDialog("Are you sure you want to resign yourself from this task?", "Resign from task");
				message.Commands.Add(new UICommand(
					"Yes",
					command => ResignProfileFromTask()));

				message.Commands.Add(new UICommand(
					"No"));

				message.DefaultCommandIndex = 0;
				message.CancelCommandIndex = 1;

				await message.ShowAsync();
			}
		}

		async private void MarkAsCompleted()
		{
			if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskMaster)
			{
				if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted || SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Completed)
				{
					MessageDialog messageToUser = new MessageDialog("You have already marked the task as completed	.", "Mark task as Completed");
					await messageToUser.ShowAsync();
				}
				else
				{
					MessageDialog messageToMaster = new MessageDialog("Are you sure the task has been completed to your satisfaction?", "Mark task as Completed");
					messageToMaster.Commands.Add(new UICommand(
						"Yes",
						command => MarkAsCompletedTaskMaster()));

					messageToMaster.Commands.Add(new UICommand(
						"No"));

					messageToMaster.DefaultCommandIndex = 0;
					messageToMaster.CancelCommandIndex = 1;

					await messageToMaster.ShowAsync();
				}
			}
			else if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskFetcher)
			{
				if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted || SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Completed)
				{
					MessageDialog messageToUser = new MessageDialog("You have already marked the task as completed.",
						"Mark task as Completed");
					await messageToUser.ShowAsync();
				}
				else
				{
					MessageDialog message = new MessageDialog("Are you sure you have completed the task?", "Mark task as Completed");
					message.Commands.Add(new UICommand(
						"Yes",
						command => MarkAsCompletedFetcher()));

					message.Commands.Add(new UICommand(
						"No"));

					message.DefaultCommandIndex = 0;
					message.CancelCommandIndex = 1;

					await message.ShowAsync();
				}

			}
			else
			{
				MessageDialog message = new MessageDialog("What are you doing? You are not assigned to the task, you are not the taskmaster, stop trying to fool the system!", "Mark task as Completed");
				await message.ShowAsync();
			}
		}

		async private void AddComment()
		{
			if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskMaster || ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskFetcher)
			{
				MessageDialog message = new MessageDialog("Are you sure you want to leave this comment?", "Leava a Comment");
				message.Commands.Add(new UICommand(
					"Yes",
					command => AddCommentToTask()));

				message.Commands.Add(new UICommand(
					"No"));

				message.DefaultCommandIndex = 0;
				message.CancelCommandIndex = 1;

				await message.ShowAsync();
			}
			else
			{
				MessageDialog message = new MessageDialog("You can only leave a comment if you are assigned to the task or the taskmaster of the task.", "Leava a Comment");
				await message.ShowAsync();
			}
		}
		#endregion

		#region SupportingMethods
		async private void SaveChangesForTask()
		{
			try
			{
				SelectedTask.TaskFee = Convert.ToDecimal(TaskFee);
				SelectedTask.TaskItemPrice = Convert.ToDecimal(ItemPrice);

				UpdateTask();

				await Task.Delay(1000);
				SuccessMessage = "Visible";
				await Task.Delay(5000);
				SuccessMessage = "Collapsed";
			}
			catch (Exception)
			{
				ErrorHandler.UpdatingError(new TaskModel());
			}
		}

		public void AssignProfileToTask()
		{
			SelectedTask.FK_TaskFetcher = ProfileHandler.CurrentLoggedInProfile.ProfileId;
			UpdateTask();
			Fetcher = ProfileHandler.CurrentLoggedInProfile;
			OnPropertyChanged("FetcherName");
		}

		public void ResignProfileFromTask()
		{
			SelectedTask.FK_TaskFetcher = null;
			UpdateTask();
			Fetcher = null;
			OnPropertyChanged("FetcherName");
		}

		public void UpdateTask()
		{
			TaskHandler.Update(SelectedTask);
		}

		public void MarkAsCompletedFetcher()
		{
			if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted)
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Completed;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.Completed;
			}
			else
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.FetcherCompleted;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.FetcherCompleted;
			}
		}

		public void MarkAsCompletedTaskMaster()
		{
			if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted)
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Completed;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.Completed;
			}
			else
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.TaskMasterCompleted;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.TaskMasterCompleted;
			}
		}

		async private void AddCommentToTask()
		{
			try
			{
				MessageHandler.CreateTaskComment(SelectedTask, CommentToLeave, ProfileHandler.CurrentLoggedInProfile);
				await Task.Delay(1000);
				CommentsForTask = MessageHandler.GetTaskComments(SelectedTask).Result.ToObservableCollection();
				CommentToLeave = "";
				SuccessMessage = "Visible";
				await Task.Delay(5000);
				SuccessMessage = "Collapsed";
			}
			catch (Exception)
			{
				ErrorHandler.CreatingError(new CommentModel());
			}
		}
		#endregion
		#endregion

		#region Properties
		public string CommentToLeave
		{
			get { return _commentToLeave; }
			set
			{
				_commentToLeave = value;
				OnPropertyChanged();
			}
		}
		public ObservableCollection<CommentModel> CommentsForTask
		{
			get { return _commentsForTask; }
			set
			{
				_commentsForTask = value;
				OnPropertyChanged();
			}
		}
		public TaskHandler.TaskStatus TaskStatus
		{
			get { return _taskStatus; }
			set
			{
				_taskStatus = value;
				OnPropertyChanged();
			}
		}

		public string FetcherName
		{
			get
			{
				if (Fetcher != null)
				{
					return Fetcher.ProfileName;
				}
				else
				{
					return "There is no fetcher assigned";
				}

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
