using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
	//Author: Kristinn Þór Jónsson
	internal class TaskDetailViewModel
	{
		#region Fields
		public TaskHandler TaskHandler { get; set; }
		public ProfileHandler ProfileHandler { get; set; }

		public ProfileModel Taskmaster { get; set; }
		public ProfileModel Fetcher { get; set; }
		public TaskModel SelectedTask { get; set; }
		public TaskHandler.TaskStatus TaskStatus { get; set; }

		public ICommand SaveChangesCommand { get; set; }
		public ICommand AssignToTaskCommand { get; set; }
		public ICommand ResignFromTaskCommand { get; set; }
		public ICommand MarkAsCompletedCommand { get; set; }
		public ICommand SuspendTaskCommand { get; set; }
		#endregion

		#region Constructor
		public TaskDetailViewModel()
		{
			TaskHandler = TaskHandler.GetInstance();
			ProfileHandler = ProfileHandler.GetInstance();

			SaveChangesCommand = new RelayCommand(SaveChanges);
			AssignToTaskCommand = new RelayCommand(AssignToTask);
			ResignFromTaskCommand = new RelayCommand(ResignFromTask);
			MarkAsCompletedCommand = new RelayCommand(MarkAsCompleted);
			SuspendTaskCommand = new RelayCommand(SuspendTask);

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

		public void ResignFromTask()
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

		private void MarkAsCompleted()
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

				messageToMaster.ShowAsync();
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

				message.ShowAsync();
			}
		}

		private void SuspendTask()
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
		#endregion

		#region SupportingMethods
		private void SuspendThisTask()
		{
			SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Removed;
			UpdateTask();
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

		public void UpdateTask()
		{
			try
			{
				TaskHandler.Update(SelectedTask);
			}
			catch (Exception e)
			{
				ErrorHandler.GetInstance().UpdatingError(new TaskModel());
			}
		}

		public void MarkAsCompletedFetcher()
		{
			if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted)
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Completed;
				UpdateTask();
			}
			else
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.FetcherCompleted;
				UpdateTask();
			}
		}

		public void MarkAsCompletedTaskMaster()
		{
			if (SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted)
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.Completed;
				UpdateTask();
			}
			else
			{
				SelectedTask.FK_TaskStatus = (int)TaskHandler.TaskStatus.TaskMasterCompleted;
				UpdateTask();
			}
		}
		#endregion
		#endregion
	}
}
