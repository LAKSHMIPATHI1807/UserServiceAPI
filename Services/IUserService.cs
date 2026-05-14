using UserServiceAPI.DTOs;

namespace UserServiceAPI.Services
{
    public interface IUserService
    {
        Task <List<ReadUserDto>> GetAllUsersAsync();
        Task <ReadUserDto> GetUserByIdAsync(int id);
        Task<ReadUserDto> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task <LoginUserDto> LoginUserAsync(LoginUserDto loginUserDto);
        Task DeleteUserAsync(int id);
    }
}
