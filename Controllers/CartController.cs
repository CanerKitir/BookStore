using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace deneme.CartControllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("/add/{userId}")]
        [Authorize(Roles = "Admin,Seller,Customer")]
        public IActionResult AddToCart(int userId, [FromBody] CartItemDto cartItemDto)
        {
            var UserId = User.FindFirst("id").Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(UserId, out var tokenUserId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            if (userId != tokenUserId && userRole != "Admin")
            {
                return Forbid();
            }
            try
            {
                _cartService.AddToCart(userId, cartItemDto.BookId, cartItemDto.Quantity);
                return Ok("Item added to cart successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,Seller,Customer")]
        public IActionResult GetCartItems(int userId)
        {
            var UserId = User.FindFirst("id").Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(UserId, out var tokenUserId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            if (userId != tokenUserId && userRole != "Admin")
            {
                return Forbid();
            }
            try
            {
                var cartItems = _cartService.GetCartItems(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult GetAllCarts()
        {
            try
            {
                var carts = _cartService.GetAllCarts();
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("/remove/{userId}")]
        [Authorize(Roles = "Admin,Seller,Customer")]
        public IActionResult RemoveFromCart(int userId, [FromBody] CartItemDto cartItemDto)
        {
            var UserId = User.FindFirst("id").Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(UserId, out var tokenUserId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            if (userId != tokenUserId && userRole != "Admin")
            {
                return Forbid();
            }
            try
            {
                _cartService.RemoveFromCart(userId, cartItemDto.BookId, cartItemDto.Quantity);
                return Ok("Item quantity decreased in cart successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/delete/{userId}/{bookId}")]
        [Authorize(Roles = "Admin,Seller,Customer")]
        public IActionResult DeleteFromCart(int userId, int bookId)
        {
            var UserId = User.FindFirst("id").Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(UserId, out var tokenUserId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            if (userId != tokenUserId && userRole != "Admin")
            {
                return Forbid();
            }
            try
            {
                _cartService.DeleteFromCart(userId, bookId);
                return Ok("Item removed from cart successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
