using Client.Models.User;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync();
        Task<StorageInfoDto> GetStorageInfoAsync();
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
    }
}