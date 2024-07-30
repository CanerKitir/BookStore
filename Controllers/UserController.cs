using Microsoft.AspNetCore.Mvc;
using deneme.UserServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace deneme.UserControllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<User>> Get()
        {
            return _userService.GetAllUsers();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Seller,Customer")]
        public ActionResult<User> Get(int id)
        {
            var userId = User.FindFirst("id").Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userId, out var tokenUserId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            if (id != tokenUserId && userRole != "Admin")
            {
                return Forbid();
            }
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] UserDto user)
        {
            _userService.AddUser(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] registerDto user)
        {
            if (_userService.GetUserByUsername(user.username) != null)
            {
                return BadRequest("Username is already taken");
            }
            User createdUser = _userService.registerUser(user);
            return CreatedAtAction(nameof(Get), new { id = createdUser.Id });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] loginDto loginUser)
        {
            if (_userService.VerifyUserCredentials(loginUser.username, loginUser.password, out var user))
            {

                return Ok(_userService.GenerateToken(loginUser.username));
            }

            return Unauthorized("Invalid username or password");
        }

        [HttpPut("/Update-User/")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put([FromBody] UserDto user)
        {
            var existingUser = _userService.GetUserById(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _userService.UpdateUser(user);
            return Ok("User updated successfully");
        }

        [HttpPut("/Update-Account/")]
        [Authorize(Roles = "Admin,Seller,Customer")]
        public IActionResult Put([FromBody] UserSecurityDto user)
        {
            var userId = User.FindFirst("id").Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userId, out var tokenUserId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            if (user.Id != tokenUserId && userRole != "Admin")
            {
                return Forbid();
            }

            var existingUser = _userService.GetUserById(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _userService.UpdateUserSecurity(user);
            return Ok("User updated successfully");
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.DeleteUser(id);
            return Ok("User deleted successfully");
        }
    }
}
