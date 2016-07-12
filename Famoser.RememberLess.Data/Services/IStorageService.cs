using System.Threading.Tasks;

namespace Famoser.RememberLess.Data.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Get cached Data (saved on every Device)
        /// </summary>
        /// <returns></returns>
        Task<string> GetCachedData();

        /// <summary>
        /// Get old cached Data (to transfer into new format)
        /// </summary>
        /// <returns></returns>
        Task<string> GetOldCachedData(int dataVersion);

        /// <summary>
        /// Get User informations, the same for all devices of a single User
        /// </summary>
        /// <returns></returns>
        Task<string> GetUserInformations();
        
        Task<bool> SetCachedData(string data);
        Task<bool> SetUserInformations(string info);

        Task<bool> ResetApplication();
    }
}
