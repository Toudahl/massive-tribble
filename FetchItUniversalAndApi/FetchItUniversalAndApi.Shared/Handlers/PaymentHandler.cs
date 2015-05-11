using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    // Author: Morten Toudahl
    class PaymentHandler: IPaymentProvider
    {
        private IPaymentProvider _provider;
        private static PaymentHandler _handler;
        private static readonly Object _lockObject = new object();

        private PaymentHandler()
        {
            _provider = new ExamplePaymentProvider();
        }

        /// <summary>
        /// Thread safe singleton instantiation. Default paymentprovider: ExamplePaymentProvider
        /// </summary>
        /// <returns><see cref="PaymentHandler"/></returns>
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

        /// <summary>
        /// Use this to change the payment provider.
        /// </summary>
        /// <param name="provider">new instance of a IPaymentProvider type</param>
        public void SetPaymentProvider(IPaymentProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Cancel the payment reservation associated with a task
        /// </summary>
        /// <param name="obj">The task whose payment reservation should be cancelled.</param>
        /// <returns>Bool, based on the success of the request.</returns>
        public Task<bool> CancelReservation(TaskModel obj)
        {
            return _provider.CancelReservation(obj);
        }

        /// <summary>
        /// Deposit money into the bank account of the TaskMaster
        /// </summary>
        /// <param name="obj">The task that contains the nessesary information</param>
        /// <returns>Bool, based on the success of the request.</returns>
        public Task<bool> Deposit(TaskModel obj)
        {
            return _provider.Deposit(obj);
        }

        /// <summary>
        /// Reserve money on from the TaskMaster
        /// </summary>
        /// <param name="obj">The task that contains the nessesary information</param>
        /// <returns>Bool, based on the success of the request.</returns>
        public Task<bool> Reserve(TaskModel obj)
        {
            return _provider.Reserve(obj);
        }

        /// <summary>
        /// Withdraw money money from TaskMaster, take our cut and send the rest to the TaskFetcher
        /// </summary>
        /// <param name="obj">The task that contains the nessesary information</param>
        /// <returns>Bool, based on the success of the request.</returns>
        public Task<bool> Withdraw(TaskModel obj)
        {
            return _provider.Withdraw(obj);
        }
    }
}
