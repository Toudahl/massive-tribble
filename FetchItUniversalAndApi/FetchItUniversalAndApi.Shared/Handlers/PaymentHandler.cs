using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class PaymentHandler: IPaymentProvider
    {
        private IPaymentProvider _provider;
        private static PaymentHandler _handler;
        private static Object _lockObject = new object();

        private PaymentHandler()
        {
            
        }

        // Write in the comment for this method, that people must remember to set the provider too.
        public static PaymentHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new PaymentHandler();
                }
                return _handler;
            }
        }

        public void SetPaymentProvider(IPaymentProvider provider)
        {
            _provider = provider;
        }

        public void CancelReservation(TaskModel obj)
        {
            _provider.CancelReservation(obj);
        }

        public void Deposit(TaskModel obj)
        {
            _provider.Deposit(obj);
        }

        public void Reserve(TaskModel obj)
        {
            _provider.Reserve(obj);
        }

        public void Withdraw(TaskModel obj)
        {
            _provider.Withdraw(obj);
        }
    }
}
