using Core.DTOs;
using Core.Models;

namespace Core.Interfaces
{
    public interface IAuth
    {
        Task<AuthDTO> LoginAsync(LoginDTO loginDTO);
        Task<AuthDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthDTO> RefreshTokenAsync(string token);
        Task<AuthDTO> UpdateUserAsync(string userId, UpdateUserDTO updatedUser);
        Task<UserDTO> GetUserByIDAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<AuthDTO> InstructorRegisterAsync(InstructorRegisterDTO instructorDto);
        Task<InstructorUser> GetInstructorByIdAsync(string instructorId);

    }
}
