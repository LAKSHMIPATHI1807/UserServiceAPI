using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using UserServiceAPI.DTOs;
using UserServiceAPI.Entities;
using UserServiceAPI.Repositories;

namespace UserServiceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(IUserRepository repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }
            await _repository.DeleteUserAsync(id);
        }

        public async Task<List<ReadUserDto>> GetAllUsersAsync()
        {
            var users =await _repository.GetAllUsersAsync();
            return _mapper.Map<List<ReadUserDto>>(users);
        }

        public async Task<ReadUserDto> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return _mapper.Map<ReadUserDto>(user);
        }

        public async Task<LoginUserDto> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var user = await _repository.GetUserByNameAsync(loginUserDto.UserName);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid password");
            }
            var token = GenerateJwtToken(user);
            return new LoginUserDto
            {
                UserName = user.UserName,
                Password = token // Return the JWT token instead of the password
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ReadUserDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            if (string.IsNullOrWhiteSpace(registerUserDto.UserName) || string.IsNullOrWhiteSpace(registerUserDto.Password))
            {
                throw new Exception("Username and password cannot be empty.");
            }
            if (!Enum.IsDefined(typeof(AllowedRoles), registerUserDto.Role))
            {
                throw new Exception($"Role must be Admin, Doctor, Patient.");
            }
            var esistingUser = await _repository.GetUserByNameAsync(registerUserDto.UserName);
            if (esistingUser != null)
            {
                throw new Exception("User already exists.");
            }
            var user = _mapper.Map<User>(registerUserDto);
            user.Password = _passwordHasher.HashPassword(user, registerUserDto.Password);
            user.Role = registerUserDto.Role;

            var createdUser = await _repository.CreateUserAsync(user);
            return _mapper.Map<ReadUserDto>(createdUser);
        }
    }
}
