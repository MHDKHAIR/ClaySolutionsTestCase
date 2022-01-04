using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ILockControlService
    {
        /// <summary>
        /// Open a lock on a door
        /// </summary>
        /// <param name="key">8 characters key</param>
        /// <param name="timeOutToClose">10 seconds</param>
        /// <returns></returns>
        Task<bool> OpenLock(string key, int timeOutToClose = 10);
        /// <summary>
        /// Close a lock on a door
        /// </summary>
        /// <param name="key">8 characters key</param>
        /// <returns></returns>
        Task<bool> CloseLock(string key);
    }
}
