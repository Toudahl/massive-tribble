using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    // Author : Morten Toudahl
    class TaskCreationViewModel
    {
        private TaskHandler _th;
        private ProfileHandler _ph;
        private ICommand _createTaskCommand;
        private string _validationErrors;
        //private DateTime Deadline;
        //private ObservableCollection<TaskLocationInfoModel> _taskLocations;

        public TaskCreationViewModel()
        {
            _th = TaskHandler.GetInstance();
            _ph = ProfileHandler.GetInstance();
            EndPointAddress = _ph.CurrentLoggedInProfile.ProfileAddress;
            Time = DateTime.Now.TimeOfDay;
            Date = DateTime.Now.Date;
        }

        public string Description { get; set; }

        public TimeSpan Time { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ItemPrice { get; set; }

        public string Fee { get; set; }
        public string EndPointAddress { get; set; }

        // TODO: properly implement task locations - also update the TaskHandler.Create() method
        //public ObservableCollection<TaskLocationInfoModel> TaskLocations
        //{
        //    get { return _taskLocations; }
        //    set { _taskLocations = value; }
        //}


        public ICommand CreateTaskCommand
        {
            get
            {
                if (_createTaskCommand == null)
                {
                    _createTaskCommand = new RelayCommand(CreateTask);
                }
                return _createTaskCommand;
            }
        }

        private void CreateTask()
        {
            if (Validation())
            {
                _th.Create(new TaskModel
                {
                    TaskDeadline = DateTimeCustom.TimeSpanAndDateOffsetToDateTime(Date, Time),
                    TaskDescription = Description,
                    TaskItemPrice = Convert.ToDecimal(ItemPrice),
                    TaskFee = Convert.ToDecimal(Fee),
                    TaskEndPointAddress = EndPointAddress,
                    //TaskLocationInfos = new ObservableCollection<TaskLocationInfoModel>(TaskLocations)

                });
            }
            else
            {
                var msg = new MessageDialog(_validationErrors).ShowAsync();
            }
        }

        private bool Validation()
        {
            _validationErrors = "";

            if (Description == null)
            {
                _validationErrors += "\nYou must write a description for the task.";
            }

            if (Convert.ToDecimal(ItemPrice) < 0)
            {
                _validationErrors += "\nThe item price cannot be negative.";
            }

            if (Convert.ToDecimal(Fee) <= 0)
            {
                if (Convert.ToDecimal(Fee) < 0)
                {
                    _validationErrors += "\nThe fee cannot be negative.";
                }
                else if (Convert.ToDecimal(Fee) == 0)
                {
                    _validationErrors += "\nThe fee cannot be zero";
                }
            }

            if (EndPointAddress == "")
            {
                _validationErrors +=
                    "\nIf you do not enter an end point for the task, the fetcher will not be able to complete the task";
            }

            if (_validationErrors == "")
            {
                return true;
            }
            return false;
        }
    }
}
