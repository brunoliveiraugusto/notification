using System.Threading.Tasks;

namespace Notification.Messenger.Interfaces
{
    public interface IMessengerService<T> where T : class
    {
        Task<bool> Deliver(T message);
    }
}
