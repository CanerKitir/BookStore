using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using denemeData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace deneme.UserServices
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public User AddUser(UserDto userDto)
        {
            User user = new User
            {
                Username = userDto.Username,
                Password = PasswordHelper.HashPassword(userDto.Password),
                Role = userDto.Role,
                Balance = userDto.Balance,
                UpdateTime = DateTime.Now,
                CreatedTime = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;

        }
        public User registerUser(registerDto user)
        {
            if (!user.password.Equals(user.registerPassword))
            {
                return BadRequest("The passwords do not match. Please ensure both passwords are identical.");
            }

            User user2 = new User
            {
                Balance = 100,
                Role = "Customer",
                Password = PasswordHelper.HashPassword(user.registerPassword),
                Username = user.username,
                Cart = new Cart(),
                UpdateTime = DateTime.Now,
                CreatedTime = DateTime.Now

            };
            _context.Users.Add(user2);
            _context.SaveChanges();
            return user2;

        }

        private User BadRequest(string v)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserDto user)
        {
            var existingUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Username = user.Username;
                existingUser.Password = PasswordHelper.HashPassword(user.Password);
                existingUser.Balance = user.Balance;
                existingUser.Role = user.Role;
                existingUser.UpdateTime = DateTime.Now;
            }
            _context.Users.Update(existingUser);
            _context.SaveChanges();
        }

        public void UpdateUserSecurity(UserSecurityDto user)
        {
            var existingUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Username = user.Username;
                existingUser.Password = PasswordHelper.HashPassword(user.Password);
                existingUser.Balance = user.Balance;
                existingUser.UpdateTime = DateTime.Now;
            }
            _context.Users.Update(existingUser);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public bool VerifyUserCredentials(string username, string password, out User user)
        {
            user = GetUserByUsername(username);
            if (user == null)
            {
                return false;
            }

            return PasswordHelper.VerifyPassword(password, user.Password);
        }

        public string GenerateToken(string username)
        {
            var user = GetUserByUsername(username);
            return GenerateJwtToken(user);
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);

        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("id", user.Id.ToString())
        };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
