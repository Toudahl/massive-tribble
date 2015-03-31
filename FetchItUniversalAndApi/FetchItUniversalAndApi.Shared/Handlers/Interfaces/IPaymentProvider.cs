using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers.Interfaces
{
    interface IPaymentProvider
    {
        void CancelReservation(TaskModel obj);
        void Deposit(TaskModel obj);
        void Reserve(TaskModel obj);
        void Withdraw(TaskModel obj);
    }
}
