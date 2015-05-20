using System.Threading.Tasks;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers.Interfaces
{
    // Author : Morten Toudahl
    interface IPaymentProvider
    {
        Task<bool> CancelReservation(TaskModel obj);
        Task<bool> Deposit(TaskModel obj);
        Task<bool> Reserve(TaskModel obj);
        Task<bool> Withdraw(TaskModel obj);
    }
}
