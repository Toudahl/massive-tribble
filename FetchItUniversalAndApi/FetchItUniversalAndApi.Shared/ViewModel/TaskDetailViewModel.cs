﻿using System;
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


		//These fields are mainly used to help with synchronizing the buttons on the
		//task detail page
		public bool MarkedCompleted { get; set; }
		public bool Assigned { get; set; }
		public bool Resigned { get; set; }
		#endregion

		#region Constructor
		public TaskDetailViewModel()
		{
			//This success message string is binded two-way in the View, it makes a textbox
			//visible or collapsed, telling the user that something was successful 
			//(adding comment, saving changes, suspending task)

			SuccessMessage = "Collapsed";

			TaskHandler = TaskHandler.GetInstance();
			ProfileHandler = ProfileHandler.GetInstance();
			ProfileHandler.GetAllProfiles();

			SaveChangesCommand = new RelayCommand(SaveChanges);
			AssignToTaskCommand = new RelayCommand(AssignToTask);
			ResignFromTaskCommand = new RelayCommand(ResignFromTask);
			MarkAsCompletedCommand = new RelayCommand(MarkAsCompleted);
			AddCommentCommand = new RelayCommand(AddComment);

			//Needs to check if TaskHandler.SelectedTask is null, or there is a "Reference not set to an Object"
			//error on the TaskDetailPage
			if (TaskHandler.SelectedTask != null)
			{
				SelectedTask = TaskHandler.SelectedTask;

				//Sets the taskmaster
				var profileOfTm =
					ProfileHandler.AllProfiles.Where(profile => profile.ProfileId == SelectedTask.FK_TaskMaster).Select(profile => profile).ToList().First();
				Taskmaster = profileOfTm;

				//Sets the Fetcher (Needs this check, or else a Exception is thrown)
				if (SelectedTask.FK_TaskFetcher != null)
				{
					var profileOfFetcher = ProfileHandler.AllProfiles.Where(profile => profile.ProfileId == SelectedTask.FK_TaskFetcher).Select(profile => profile).ToList().First();
					if (profileOfFetcher != null)
					{
						Fetcher = profileOfFetcher;
					}
				}

				//Sets the feedbacks (if any)
				FeedbackModel feedbackForThisTask = new FeedbackModel();
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

				//Here the .Feedbacks property is used, only so that the view knows if it
				//should show the 'Create Feedback' button
                //TODO: Fix this, so it does not deppend no non-functional navigational properties.
                //if (feedbackForThisTask != null)
                //{
                //    if (SelectedTask.Feedbacks.Count == 0)
                //    {
                //        SelectedTask.Feedbacks.Add(feedbackForThisTask);
                //    }
                //}

				TaskStatus = (TaskHandler.TaskStatus)SelectedTask.FK_TaskStatus;

				//Adds all the comments through the MessageHandler
				CommentsForTask = MessageHandler.GetTaskComments(SelectedTask).ToObservableCollection();

				ItemPrice = SelectedTask.TaskItemPrice.ToString();
				TaskFee = SelectedTask.TaskFee.ToString();
			}
		}

		#endregion

		#region Methods
		#region MessageMethods

		/// <summary>
		/// A method that shows the user a MessageDialog, giving him the option to save the changes he made to his task.
		/// </summary>
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

		/// <summary>
		/// A method that shows the user a MessageDialog, making him confirm that he wants to assign himself to the selected task.
		/// </summary>
		async public void AssignToTask()
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

		/// <summary>
		/// A method that shows the user a MessageDialog, making him confirm that he wants to resign himself from the selected task.
		/// </summary>
		async public void ResignFromTask()
		{
			if (SelectedTask.FK_TaskStatus != (int)TaskHandler.TaskStatus.Active)
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

		/// <summary>
		/// A method that shows the user a MessageDialog, making him confirm that he is sure that the selected task has been completed.
		/// </summary>
		async private void MarkAsCompleted()
		{
			if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskMaster)
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
			else if (ProfileHandler.CurrentLoggedInProfile.ProfileId == SelectedTask.FK_TaskFetcher)
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
			else
			{
				MessageDialog message = new MessageDialog("What are you doing? You are not assigned to the task, you are not the taskmaster, stop trying to fool the system!", "Mark task as Completed");
				await message.ShowAsync();
			}
		}

		/// <summary>
		/// A method that shows the user a MessageDialog, making him confirm that he wants to add the provided comment to the selected task.
		/// </summary>
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
		/// <summary>
		/// This method saves the changes for the task, and calls the UpdateTask method.
		/// </summary>
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

		/// <summary>
		/// This method assigns the logged in profile as Fetcher for the selected task, and calls the UpdateTask method. Also sends notification to the Taskmaster.
		/// </summary>
		public void AssignProfileToTask()
		{
			SelectedTask.FK_TaskFetcher = ProfileHandler.CurrentLoggedInProfile.ProfileId;
			UpdateTask();

			//Used to make button dissappear
			Assigned = true;
			MessageHandler.SendNotification("Profile: '" + ProfileHandler.CurrentLoggedInProfile.ProfileName + "' just assigned himself as a fetcher for a task of yours. The task id is '" + SelectedTask.TaskId + "'.", Taskmaster);
			Fetcher = ProfileHandler.CurrentLoggedInProfile;
			OnPropertyChanged("FetcherName");
		}

		/// <summary>
		/// This method resigns the logged in profile from the selected task, and calls the UpdateTask method. Also sends notification to the Taskmaster.
		/// </summary>
		public void ResignProfileFromTask()
		{
			SelectedTask.FK_TaskFetcher = null;
			Fetcher = null;
			UpdateTask();

			//Used to make button dissappear
			Resigned = true;
			MessageHandler.SendNotification("Fetcher: '" + ProfileHandler.CurrentLoggedInProfile.ProfileName + "' just resigned himself from a task of yours. The task id is " + SelectedTask.TaskId + ".", Taskmaster);
			OnPropertyChanged("FetcherName");
		}

		/// <summary>
		/// A method that calls the Taskhandler to Update the selected task.
		/// </summary>
		public void UpdateTask()
		{
			TaskHandler.Update(SelectedTask);
		}

		/// <summary>
		/// This method changes the status of the selected task to "FetcherCompleted" and calls the updateTask method. If the task is already "MasterCompleted" the status will change to "Completed". Also sends notification to relative profiles.
		/// </summary>
		public void MarkAsCompletedFetcher()
		{
			MessageHandler.SendNotification("Fetcher: '" + ProfileHandler.CurrentLoggedInProfile.ProfileName + "' just marked task '" + SelectedTask.TaskId + "' as completed.", Taskmaster);
			MarkedCompleted = true;

			if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted)
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Completed;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.Completed;
				MessageHandler.SendNotification("The task '" + SelectedTask.TaskDescription.Substring(0, 10) + "' has successfully been completed.", ProfileHandler.CurrentLoggedInProfile);
				MessageHandler.SendNotification("The task '" + SelectedTask.TaskDescription.Substring(0, 10) + "' has successfully been completed.", Taskmaster);
			}
			else
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.FetcherCompleted;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.FetcherCompleted;
			}
		}

		/// <summary>
		/// This method changes the status of the selected task to "TaskmasterCompleted" and calls the updateTask method. If the task is already "FetcherCompleted" the status will change to "Completed". Also sends notification to relative profiles.
		/// </summary>
		public void MarkAsCompletedTaskMaster()
		{
			MessageHandler.SendNotification("Taskmaster: '" + Taskmaster.ProfileName + "' just marked task '" + SelectedTask.TaskId + "' as completed.", Fetcher);
			MarkedCompleted = true;

			if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted)
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Completed;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.Completed;
				MessageHandler.SendNotification("The task '" + SelectedTask.TaskId + "' has successfully been completed.", ProfileHandler.CurrentLoggedInProfile);
				MessageHandler.SendNotification("The task '" + SelectedTask.TaskId + "' has successfully been completed.", Fetcher);
			}
			else
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.TaskMasterCompleted;
				UpdateTask();
				TaskStatus = TaskHandler.TaskStatus.TaskMasterCompleted;
			}
		}

		/// <summary>
		/// This method uses the Messagehandler to POST a new comment to the database, and then refreshes the comments listview. Also sends notifications to relative profiles.
		/// </summary>
		async private void AddCommentToTask()
		{
			try
			{
				MessageHandler.CreateTaskComment(SelectedTask, CommentToLeave, ProfileHandler.CurrentLoggedInProfile);

				//Only TM or Fetcher can comment, this code manages sending notifications to the right user
				if (Fetcher != null)
				{
					if (Fetcher.ProfileId == ProfileHandler.CurrentLoggedInProfile.ProfileId)
					{
						MessageHandler.SendNotification("Fetcher: '" + ProfileHandler.CurrentLoggedInProfile.ProfileName + "' just added a comment on task '" + SelectedTask.TaskId + "'.", Taskmaster);
					}
					else
					{
						MessageHandler.SendNotification("Taskmaster: '" + ProfileHandler.CurrentLoggedInProfile.ProfileName + "' just added a comment on task '" + SelectedTask.TaskId + "'.", Fetcher); 
					}
				}
				
				await Task.Delay(1000);
				CommentsForTask = MessageHandler.GetTaskComments(SelectedTask).ToObservableCollection();
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
				return "None";
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
