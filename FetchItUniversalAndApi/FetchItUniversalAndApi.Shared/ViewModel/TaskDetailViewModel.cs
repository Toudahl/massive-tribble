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
		private ObservableCollection<CommentModel> _commentsForTask;
		private string _commentToLeave;
		private string _successMessage;
		private TaskHandler.TaskStatus _taskStatus;
		private string _updateSuccessMessage;

		#region Fields
		public TaskHandler TaskHandler { get; set; }
		public ProfileHandler ProfileHandler { get; set; }

		public ProfileModel Taskmaster { get; set; }
		public ProfileModel Fetcher { get; set; }
		public TaskModel SelectedTask { get; set; }

		public ICommand SaveChangesCommand { get; set; }
		public ICommand AssignToTaskCommand { get; set; }
		public ICommand ResignFromTaskCommand { get; set; }
		public ICommand MarkAsCompletedCommand { get; set; }
		public ICommand SuspendTaskCommand { get; set; }
		public ICommand AddCommentCommand { get; set; }
		public bool Suspended { get; set; }

		public string UpdateSuccessMessage
		{
			get { return _updateSuccessMessage; }
			set
			{
				_updateSuccessMessage = value;
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

		#region Constructor
		public TaskDetailViewModel()
		{
			SuccessMessage = "Collapsed";
			UpdateSuccessMessage = "Collapsed";

			TaskHandler = TaskHandler.GetInstance();
			ProfileHandler = ProfileHandler.GetInstance();

			SaveChangesCommand = new RelayCommand(SaveChanges);
			AssignToTaskCommand = new RelayCommand(AssignToTask);
			ResignFromTaskCommand = new RelayCommand(ResignFromTask);
			MarkAsCompletedCommand = new RelayCommand(MarkAsCompleted);
			SuspendTaskCommand = new RelayCommand(SuspendTask);
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

				if (feedbackForThisTask != null)
				{
					SelectedTask.Feedbacks.Add(feedbackForThisTask);
				}

				TaskStatus = (TaskHandler.TaskStatus)SelectedTask.FK_TaskStatus;

				CommentsForTask = MessageHandler.GetTaskComments(SelectedTask).Result.ToObservableCollection();
			}
		}

		private void AddComment()
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

				message.ShowAsync();
			}
			else
			{
				MessageDialog message = new MessageDialog("You can only leave a comment if you are assigned to the task or the taskmaster of the task.", "Leava a Comment");
				message.ShowAsync();
			}
		}

		#endregion

		#region Methods
		#region MessageMethods
		public void SaveChanges()
		{
			MessageDialog message = new MessageDialog("Are you sure you want to save the changes?", "Update task");
			message.Commands.Add(new UICommand(
				"Yes",
				command => UpdateTask()));

			message.Commands.Add(new UICommand(
				"No"));

			message.DefaultCommandIndex = 0;
			message.CancelCommandIndex = 1;

			message.ShowAsync();
		}

		public void AssignToTask()
		{
			if (SelectedTask.FK_TaskFetcher == ProfileHandler.CurrentLoggedInProfile.ProfileId)
			{
				MessageDialog alreadyResignedMessage = new MessageDialog("You have already assigned yourself to this task.", "Assign to task");
				alreadyResignedMessage.ShowAsync();
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

				message.ShowAsync();
			}
		}

		public void ResignFromTask()
		{
			if (SelectedTask.FK_TaskFetcher != ProfileHandler.CurrentLoggedInProfile.ProfileId)
			{
				MessageDialog alreadyResignedMessage = new MessageDialog("You have already resigned from this task.", "Resign from task");
				alreadyResignedMessage.ShowAsync();
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

				message.ShowAsync();
			}
		}

		private void MarkAsCompleted()
		{
			if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskMaster)
			{
				if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted)
				{
					MessageDialog messageToUser = new MessageDialog("You have already marked the task as compledet.", "Mark task as Completed");
					messageToUser.ShowAsync();
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

					messageToMaster.ShowAsync();
				}
			}
			else if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskFetcher)
			{
				if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted)
				{
					MessageDialog messageToUser = new MessageDialog("You have already marked the task as compledet.",
						"Mark task as Completed");
					messageToUser.ShowAsync();
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

					message.ShowAsync();
				}

			}
			else
			{
				MessageDialog message = new MessageDialog("What are you doing? You are not assigned to the task, you are not the taskmaster, stop trying to fool the system!", "Mark task as Completed");
				message.ShowAsync();
			}
		}

		private void SuspendTask()
		{
			if (Suspended)
			{
				MessageDialog message = new MessageDialog("You have already suspended the task.", "Suspend Task");
				message.ShowAsync();
			}
			else
			{
				MessageDialog message = new MessageDialog("Are you sure you want to suspend the task? This action will remove the Task from the marketplace.", "Suspend Task");
				message.Commands.Add(new UICommand(
					"Yes",
					command => SuspendThisTask()));

				message.Commands.Add(new UICommand(
					"No"));

				message.DefaultCommandIndex = 0;
				message.CancelCommandIndex = 1;

				message.ShowAsync();
			}
		}
		#endregion

		#region SupportingMethods
		async private void SuspendThisTask()
		{
			SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Removed;
			UpdateTask();
			Suspended = true;
			await Task.Delay(1000);
		}

		public void AssignProfileToTask()
		{
			SelectedTask.FK_TaskFetcher = ProfileHandler.CurrentLoggedInProfile.ProfileId;
			UpdateTask();
		}

		public void ResignProfileFromTask()
		{
			SelectedTask.FK_TaskFetcher = null;
			UpdateTask();
		}

		async public void UpdateTask()
		{
			try
			{
				TaskHandler.Update(SelectedTask);
				await Task.Delay(1000);
				UpdateSuccessMessage = "Visible";
				await Task.Delay(5000);
				UpdateSuccessMessage = "Collapsed";
			}
			catch (Exception e)
			{
				ErrorHandler.UpdatingError(new TaskModel());
			}
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
			MessageHandler.CreateTaskComment(SelectedTask, CommentToLeave, ProfileHandler.CurrentLoggedInProfile);
			await Task.Delay(1000);
			CommentsForTask = MessageHandler.GetTaskComments(SelectedTask).Result.ToObservableCollection();
			CommentToLeave = "";
			SuccessMessage = "Visible";
			await Task.Delay(5000);
			SuccessMessage = "Collapsed";
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
		#endregion

		#region Notify Property Changed
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
