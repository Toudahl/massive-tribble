using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class ExamplePaymentProvider: IPaymentProvider
    {
        public void CancelReservation(TaskModel obj)
        {
            throw new NotImplementedException();
        }

        public void Deposit(TaskModel obj)
        {
            throw new NotImplementedException();
        }

        public void Reserve(TaskModel obj)
        {
            throw new NotImplementedException();
        }

        public void Withdraw(TaskModel obj)
        {
            throw new NotImplementedException();
        }
    }
}
