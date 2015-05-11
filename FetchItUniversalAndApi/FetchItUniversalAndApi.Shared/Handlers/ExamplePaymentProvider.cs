using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    // Author : Morten Toudahl
    class ExamplePaymentProvider: IPaymentProvider
    {
        public Task<bool> CancelReservation(TaskModel obj)
        {
            try
            {
                /*
                 * Get payment details from obj.
                 * Contacting remote payment provider
                 * 
                 * if success display message to user
                 */
                return Task.Run(() => 1==1);

            }
            catch (Exception)
            {
                return Task.Run(() => 1 == 2);
            }
        }

        public Task<bool> Deposit(TaskModel obj)
        {
            try
            {
                /*
                 * Get payment details from obj.
                 * Contacting remote payment provider
                 * 
                 * if success display message to user
                 */
                return Task.Run(() => 1 == 1);

            }
            catch (Exception)
            {
                return Task.Run(() => 1 == 2);
            }
        }

        public Task<bool> Reserve(TaskModel obj)
        {
            try
            {
                /*
                 * Get payment details from obj.
                 * Contacting remote payment provider
                 * 
                 * if success display message to user
                 */
                return Task.Run(() => 1 == 1);

            }
            catch (Exception)
            {
                return Task.Run(() => 1 == 2);
            }
        }

        public Task<bool> Withdraw(TaskModel obj)
        {
            try
            {
                /*
                 * Get payment details from obj.
                 * Contacting remote payment provider
                 * 
                 * if success display message to user
                 */
                return Task.Run(() => 1 == 1);

            }
            catch (Exception)
            {
                return Task.Run(() => 1 == 2);
            }
        }
    }
}
